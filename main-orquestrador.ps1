# Autoria: @denalth
# main-orquestrador.ps1
# Windows Optimizer v7.0.0 - Orquestrador de Terminal
#
# Carrega todos os modulos em modules/ e apresenta um menu interativo
# que despacha para as funcoes *-Interactive de cada modulo.
# Uso:  powershell -ExecutionPolicy Bypass -File main-orquestrador.ps1
#
# Requer: privilegios de Administrador (verificacao abaixo).

[CmdletBinding()]
param(
    [switch]$NoAdminCheck
)

$ErrorActionPreference = "Stop"
$script:Version = "7.0.0"

# === DIRETORIO BASE ===
$ScriptRoot = $PSScriptRoot
if (-not $ScriptRoot) { $ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path }
$ModulesDir = Join-Path $ScriptRoot "modules"

# === ELEVACAO ===
function Test-IsAdmin {
    $id = [Security.Principal.WindowsIdentity]::GetCurrent()
    (New-Object Security.Principal.WindowsPrincipal($id)).IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not $NoAdminCheck -and -not (Test-IsAdmin)) {
    Write-Host " Este script precisa de privilegios de Administrador." -ForegroundColor Yellow
    Write-Host " Reabrindo elevado..." -ForegroundColor Cyan
    $args = @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", $PSCommandPath)
    if ($NoAdminCheck) { $args += "-NoAdminCheck" }
    Start-Process powershell -Verb RunAs -ArgumentList $args
    exit
}

# === CARREGAR MODULOS ===
# utils.ps1 DEVE ser carregado primeiro (define logging e helpers usados pelos outros)
$loadOrder = @(
    "utils.ps1",
    "backup.ps1",
    "cleanup.ps1",
    "bloatwares.ps1",
    "devtools.ps1",
    "sdks.ps1",
    "energy.ps1",
    "gaming.ps1",
    "health.ps1",
    "network.ps1",
    "privacy.ps1",
    "services.ps1",
    "visuals.ps1",
    "windowsupdate.ps1",
    "wsl2.ps1",
    "profiles.ps1"
)

Write-Host " Carregando modulos..." -ForegroundColor Cyan
$loaded = 0
foreach ($mod in $loadOrder) {
    $path = Join-Path $ModulesDir $mod
    if (Test-Path $path) {
        try {
            . $path
            $loaded++
        } catch {
            Write-Host " Falha ao carregar $mod : $_" -ForegroundColor Red
        }
    } else {
        Write-Host " Modulo ausente: $mod" -ForegroundColor DarkYellow
    }
}
Write-Host " $loaded modulos carregados.`n" -ForegroundColor Green

# === INICIALIZAR LOG ===
if (Get-Command Initialize-Log -ErrorAction SilentlyContinue) {
    Initialize-Log | Out-Null
}

# === BANNER ===
function Show-Banner {
    Clear-Host
    Write-Host ""
    Write-Host "  ____ _   _ ___    __        ______ _   _ _____ " -ForegroundColor Cyan
    Write-Host " / ___| | | / __|   \ \      / / ___| | | | ____|" -ForegroundColor Cyan
    Write-Host "| |  _| |_| \__ \    \ \ /\ / / |  _| | | |  _  " -ForegroundColor Cyan
    Write-Host "| |_| |  _  |___/     \ V  V /| |_| | |_| | |___ " -ForegroundColor Cyan
    Write-Host " \____|_| |_|          \_/\_/  \____|\___/|_____|" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  Windows Optimizer v$script:Version  |  @denalth" -ForegroundColor DarkCyan
    Write-Host "  Modo Terminal (Orquestrador)`n" -ForegroundColor DarkGray
}

# === MENU PRINCIPAL ===
# Cada item aponta para a funcao *-Interactive do modulo correspondente.
# A funcao Invoke-MenuItem protege contra ausencia de modulo.
function Invoke-MenuItem([string]$FunctionName) {
    $cmd = Get-Command $FunctionName -ErrorAction SilentlyContinue
    if ($cmd) {
        & $cmd
    } else {
        Write-Host "`n O modulo que define '$FunctionName' nao foi carregado.`n" -ForegroundColor Yellow
    }
    Read-Host "`n Pressione ENTER para voltar ao menu"
}

$menu = [ordered]@{
    "1" = @{ Label = "Performance / Energia";  Fn = "Enable-UltimatePerformance-Interactive" }
    "2" = @{ Label = "Limpeza de Sistema";     Fn = "System-Cleanup-Interactive" }
    "3" = @{ Label = "Remover Bloatwares";     Fn = "Remove-Bloatwares-Interactive" }
    "4" = @{ Label = "Privacidade";            Fn = "Privacy-Interactive" }
    "5" = @{ Label = "Rede / DNS";             Fn = "Network-Interactive" }
    "6" = @{ Label = "Visuais / Temas";        Fn = "Adjust-Visuals-Interactive" }
    "7" = @{ Label = "Servicos do Windows";    Fn = "Services-Interactive" }
    "8" = @{ Label = "Windows Update";         Fn = "WindowsUpdate-Interactive" }
    "9" = @{ Label = "Dev Tools";              Fn = "Install-DevTools-Interactive" }
    "10"= @{ Label = "SDKs";                   Fn = "Install-SDKs-Interactive" }
    "11"= @{ Label = "WSL2";                   Fn = "Setup-WSL2-Interactive" }
    "12"= @{ Label = "Saude / Diagnostico";    Fn = "Health-Interactive" }
    "13"= @{ Label = "Backup / Restauracao";   Fn = "Backup-Interactive" }
    "14"= @{ Label = "Perfis (Dev/Gamer/Work)"; Fn = "Profiles-Interactive" }
}

# === LOOP DO MENU ===
while ($true) {
    Show-Banner
    Write-Host " Escolha uma categoria:`n" -ForegroundColor White
    foreach ($key in $menu.Keys) {
        $item = $menu[$key]
        Write-Host (" [{0,2}] {1}" -f $key, $item.Label) -ForegroundColor Cyan
    }
    Write-Host " [ 0] Sair`n" -ForegroundColor DarkGray

    $choice = Read-Host " Opcao"

    if ($choice -eq "0" -or $choice -eq "") {
        Write-Host "`n Ate logo! - @denalth`n" -ForegroundColor Green
        break
    }

    if ($menu.Contains($choice)) {
        Invoke-MenuItem $menu[$choice].Fn
    } else {
        Write-Host "`n Opcao invalida." -ForegroundColor Yellow
        Start-Sleep -Seconds 1
    }
}

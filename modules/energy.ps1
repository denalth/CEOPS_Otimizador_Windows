# Autoria: @denalth
# energy.ps1
# Configuracoes de energia e performance.

function Enable-UltimatePerformance-Interactive {
    Log-Info "=== PLANO DE ENERGIA ==="

    Write-Host "`n O que e o Ultimate Performance?" -ForegroundColor Yellow
    Write-Host " E um plano de energia oculto do Windows que maximiza a velocidade do" -ForegroundColor Gray
    Write-Host " processador e elimina latencias de energia. Ideal para desktops e gamers." -ForegroundColor Gray
    Write-Host " Aviso: Aumenta o consumo de energia. Nao recomendado para notebooks na bateria.`n" -ForegroundColor Gray

    Write-Host " [Q] Voltar ao Menu Principal`n" -ForegroundColor DarkGray
    $check = Read-Host " Pressione ENTER para continuar ou Q para voltar"
    if ($check -eq 'q' -or $check -eq 'Q') { return }

    Write-Host "`n [ACAO] Ativar Plano Ultimate Performance" -ForegroundColor Cyan
    Write-Host " O que vai acontecer: O Windows revelara e ativara o plano oculto de energia maxima.`n" -ForegroundColor Yellow
    if (Confirm-YesNo "Ativar Ultimate Performance?") {
        $Guid = "e9a42b02-d5df-448d-aa00-03f14749eb61"
        try {
            powercfg -duplicatescheme $Guid | Out-Null
            powercfg /S $Guid
            Log-Success "Plano Ultimate Performance ativado."
        } catch {
            Log-Warning "Nao foi possivel ativar o plano."
        }
    }

    Write-Host "`n [ACAO] Desabilitar Hibernacao" -ForegroundColor Cyan
    Write-Host " O que vai acontecer: O arquivo hiberfil.sys sera removido, liberando GBs de espaco.`n" -ForegroundColor Yellow
    if (Confirm-YesNo "Desabilitar hibernacao?") {
        powercfg /h off
        Log-Success "Hibernacao desabilitada."
    }
}

# Autoria: @denalth
# v7.1.0 - Hibernar ao fechar a tampa (notebooks/laptops com bateria ou chassis portatil).
function Enable-Hibernate-OnLidClose-Interactive {
    Log-Info "=== HIBERNAR AO FECHAR A TAMPA ==="

    # 1. Elegibilidade: so aplica em equipamentos portateis (bateria OU chassis laptop/notebook)
    if (-not (Test-IsPortableDevice)) {
        Write-Host "`n Este equipamento nao foi detectado como portatil (notebook/laptop com bateria)." -ForegroundColor Yellow
        Write-Host " A configuracao de tampa so se aplica a portateis.`n" -ForegroundColor Gray
        Read-Host " Pressione ENTER para voltar"
        return
    }

    Write-Host "`n O que isto faz:" -ForegroundColor Yellow
    Write-Host "  - Ativa a hibernacao (cria hiberfil.sys, ~40% da RAM em disco)." -ForegroundColor Gray
    Write-Host "  - Ao fechar a tampa, o PC HIBERNA tanto na tomada (AC) quanto na bateria (DC)." -ForegroundColor Gray
    Write-Host "  - Diferente de suspender, a hibernacao desliga o equipamento e salva o estado.`n" -ForegroundColor Gray

    Write-Host " [Q] Voltar ao Menu Principal`n" -ForegroundColor DarkGray
    $check = Read-Host " Pressione ENTER para continuar ou Q para voltar"
    if ($check -eq 'q' -or $check -eq 'Q') { return }

    # 2. Verifica suporte do hardware (powercfg /a)
    $available = powercfg /a 2>$null
    if ($available -notmatch '(?i)hibernation|hibernac') {
        Log-Warning "Este hardware nao suporta hibernacao (powercfg /a). Acao cancelada."
        Read-Host "`n Pressione ENTER para voltar"
        return
    }

    if (Confirm-YesNo "Ativar hibernacao ao fechar a tampa (AC + DC)?") {
        # Alias fixos do Windows para a acao da tampa
        $SubButtons = "4f971e89-eebd-4455-a8de-9e59040e7347"
        $LidAction  = "5ca83367-6e45-459f-a27b-476b1d01c936"

        # Ativa hibernacao
        powercfg /hibernate on
        Log-Success "Hibernacao ativada."

        # AC (tomada) = Hibernar (2) ao fechar a tampa
        powercfg /SETACVALUEINDEX SCHEME_CURRENT $SubButtons $LidAction 2
        # DC (bateria) = Hibernar (2) ao fechar a tampa
        powercfg /SETDCVALUEINDEX SCHEME_CURRENT $SubButtons $LidAction 2
        # Aplica o plano corrente
        powercfg /SETACTIVE SCHEME_CURRENT

        Log-Success "Feche a tampa para hibernar (tomada e bateria)."
    }
}

# Helper: detecta equipamento portatil (bateria OU chassis laptop/notebook/sub-notebook/tablet).
function Test-IsPortableDevice {
    try {
        # Bateria (Win32_Battery ou Win32_PortableBattery)
        if (Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue) { return $true }
        if (Get-CimInstance Win32_PortableBattery -ErrorAction SilentlyContinue) { return $true }

        # Chassis portatil: 9=Laptop, 10=Notebook, 14=Sub Notebook, 30=Tablet, 8=Portable, 11=Handheld
        $enclosure = Get-CimInstance Win32_SystemEnclosure -ErrorAction SilentlyContinue
        if ($enclosure) {
            $portable = 8, 9, 10, 11, 14, 30, 31, 32
            foreach ($t in $enclosure.ChassisTypes) {
                if ($portable -contains $t) { return $true }
            }
        }

        # Fallback: PCSystemType == 2 (Mobile)
        $cs = Get-CimInstance Win32_ComputerSystem -ErrorAction SilentlyContinue
        if ($cs -and $cs.PCSystemType -eq 2) { return $true }
    } catch { }
    return $false
}


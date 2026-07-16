# Autoria: @denalth
# profiles.ps1
# Módulo de Perfis de Otimização Sugeridos.

function Profiles-Interactive {
    Log-Info "=== PERFIS DE OTIMIZACAO SUGERIDOS ==="

    Write-Host "`n O que sao Perfis?" -ForegroundColor Yellow
    Write-Host " Sao combinacoes pre-definidas de configuracoes para cenarios especificos.`n" -ForegroundColor Gray

    Write-Host " Escolha o perfil ideal para voce:" -ForegroundColor Cyan
    Write-Host " [1] Modo Dev (WSL2, SDKs, Performance de Disco, Git)"
    Write-Host " [2] Modo Gamer (Baixa Latencia, HAGS, Game Mode, Sem Overlays)"
    Write-Host " [3] Modo Trabalho/Estudio (Limpeza, Privacidade, Silencio)"
    Write-Host " [0] Voltar ao Menu Principal`n" -ForegroundColor DarkGray

    $opt = Read-Host " Opcao"

    if ($opt -eq "" -or $opt -eq "0") { return }

    # Helper: garante que a funcao existe antes de chamar e avisa se faltar o modulo
    function Invoke-IfAvailable([string]$FunctionName) {
        $cmd = Get-Command $FunctionName -ErrorAction SilentlyContinue
        if ($cmd) {
            & $cmd
        } else {
            Log-Warning "Modulo que define '$FunctionName' nao esta carregado. Pulando esta etapa."
        }
    }

    switch ($opt) {
        "1" {
            if (Confirm-YesNo "Aplicar configuracoes do Modo Dev?") {
                Log-Info "Iniciando Perfil DEV..."
                Write-Progress -Activity "Aplicando Perfil DEV" -Status "Instalando ferramentas..." -PercentComplete 20
                Invoke-IfAvailable "Install-DevTools-Interactive"
                Write-Progress -Activity "Aplicando Perfil DEV" -Status "Configurando WSL2..." -PercentComplete 50
                Invoke-IfAvailable "Setup-WSL2-Interactive"
                Write-Progress -Activity "Aplicando Perfil DEV" -Status "Limpando sistema..." -PercentComplete 80
                Invoke-IfAvailable "System-Cleanup-Interactive"
                Write-Progress -Activity "Aplicando Perfil DEV" -Status "Concluido" -Completed
                Log-Success "Perfil DEV aplicado por @denalth!"
            }
        }
        "2" {
            if (Confirm-YesNo "Aplicar configuracoes do Modo Gamer?") {
                Log-Info "Iniciando Perfil GAMER..."
                Write-Progress -Activity "Aplicando Perfil GAMER" -Status "Otimizando Rede..." -PercentComplete 30
                $Interfaces = Get-ChildItem "HKLM:\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces"
                foreach ($Interface in $Interfaces) {
                    New-ItemProperty -Path $Interface.PSPath -Name "TcpAckFrequency" -Value 1 -PropertyType DWord -Force -ErrorAction SilentlyContinue | Out-Null
                    New-ItemProperty -Path $Interface.PSPath -Name "TCPNoDelay" -Value 1 -PropertyType DWord -Force -ErrorAction SilentlyContinue | Out-Null
                }
                Write-Progress -Activity "Aplicando Perfil GAMER" -Status "Configurando Windows..." -PercentComplete 60
                Invoke-IfAvailable "Gaming-Interactive"
                Write-Progress -Activity "Aplicando Perfil GAMER" -Status "Ajustando Energia..." -PercentComplete 90
                $Guid = "e9a42b02-d5df-448d-aa00-03f14749eb61"
                powercfg -duplicatescheme $Guid | Out-Null
                powercfg /S $Guid
                Write-Progress -Activity "Aplicando Perfil GAMER" -Status "Concluido" -Completed
                Log-Success "Perfil GAMER aplicado por @denalth!"
            }
        }
        "3" {
            if (Confirm-YesNo "Aplicar configuracoes do Modo Trabalho/Estudio?") {
                Log-Info "Iniciando Perfil WORK..."
                Write-Progress -Activity "Aplicando Perfil WORK" -Status "Privacidade..." -PercentComplete 30
                Invoke-IfAvailable "Privacy-Interactive"
                Write-Progress -Activity "Aplicando Perfil WORK" -Status "Limpando disco..." -PercentComplete 60
                Invoke-IfAvailable "System-Cleanup-Interactive"
                Write-Progress -Activity "Aplicando Perfil WORK" -Status "Visual/Energia..." -PercentComplete 90
                Invoke-IfAvailable "Adjust-Visuals-Interactive"
                Write-Progress -Activity "Aplicando Perfil WORK" -Status "Concluido" -Completed
                Log-Success "Perfil WORK aplicado por @denalth!"
            }
        }
        default {
            Log-Warning "Opcao invalida."
        }
    }
}

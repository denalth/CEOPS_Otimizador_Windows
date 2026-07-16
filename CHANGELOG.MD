# Autoria: @denalth
# Changelog - Windows Optimizer

## [7.0.0] - 2026-07-15 (Refatoração Arquitetural — BUILD VALIDADO)

### ⚠️ Breaking Changes
- **Removida a GUI PowerShell** (`Lancar_GUI.ps1`) — era uma duplicata da WPF C#.
- **Removido o launcher WinForms** (`WindowsOptimizerApp/Program.cs`) — a WPF agora se auto-eleva via UAC.
- **Removido o binário commitado** (`WindowsOptimizer.exe`) — agora é gerado no build.

### ✨ Adicionado
- **`main-orquestrador.ps1`**: menu de terminal que carrega todos os `modules/*.ps1` (resolve o app de terminal órfão).
- **Auto-elevação UAC**: a GUI WPF verifica privilégios de admin e se re-eleva.
- **`<LangVersion>latest</LangVersion>`** no `.csproj` (recursos C# modernos).
- **Helper `Invoke-IfAvailable`** em `profiles.ps1` (avisa quando um módulo falta).

### 🔧 Corrigido
- **#1 Self-Update**: URL agora aponta para `CEOPS_Otimizador_Windows` (antes: `otimizador_windows` inexistente).
- **#2 Triplicação de código**: eliminadas as 3 codebases duplicadas (modules + 2 GUIs → 1 WPF + modules).
- **#3 UI congela**: ações executam em `Task.Run`/`async/await` (não bloqueia mais a thread de UI).
- **#4 DNS "Ethernet" hardcoded**: agora aplica em todos os adaptadores ativos via `Get-NetAdapter`.
- **#5 Versão fora de sync**: 7.0.0 unificada em todos os arquivos + comparação via `System.Version`.
- **#6 Funções inexistentes**: `profiles.ps1` chamava `Show-SupremeGUI` e `Install-DevTools-Essentials` (corrigido).
- **#8 OneDrive path hardcoded**: detecta SysWOW64/System32/per-user + fallback `winget uninstall`.
- **#9 CleanTemp parcial**: agora recursivo (arquivos + subdiretórios).
- **#10 RecycleBin só C:**: trocado por `Clear-RecycleBin -Force` (todas as unidades).
- **#13 Mojibake**: GUI reescrita, emojis e acentuação corrigidos.

### 🗑️ Removido
- `Lancar_GUI.ps1` (GUI PowerShell duplicada)
- `WindowsOptimizerApp/Program.cs` + pasta (launcher WinForms redundante)
- `WindowsOptimizer.exe` (binário commitado)
- `WindowsOptimizerWPF/AssemblyInfo.cs` (conflitava com o gerado pelo SDK-style)

### 🏗️ Build
- Validado com .NET SDK 8: **0 erros, 0 avisos**.
- Metadados de assembly movidos do `AssemblyInfo.cs` para o `.csproj`.

---

## [6.0.4] - 2025-12-27 (Botões Completos)
### Adicionado
- **Eventos de Clique**: Todos os 14 botões de categoria agora funcionam (Dev Tools, SDKs, WSL2, Perfis).
- **15 Add_Click Events**: Cobertura total de interação na GUI WPF.
## [6.0.3] - 2025-12-27 (Fixed Visuals & Actions)
### Corrigido
- **Encoding UTF-8 BOM**: Resolvido o problema de caracteres quebrados (mojibake) em emojis e acentos.
- **Ações Reativadas**: Todas as 57 ações do sistema migradas para o novo padrão WPF estável.
- **Estabilidade WPF**: Removidos erros de parsing de Dispatcher e ScriptBlocks.
## [6.0.0] - 2025-12-27 (Interface WPF Premium)
### Nova Arquitetura
- **WPF Nativo**: Migração completa de WinForms para WPF via PowerShell dinâmico.
- **Design GitHub Dark**: Paleta de cores premium (#0D1117, #58A6FF, #3FB950).
- **Emojis Coloridos**: 🎨 Cada categoria e ação tem seu emoji temático.
- **Acentuação pt-BR**: Suporte completo a caracteres acentuados.
- **Animações de Hover**: Botões com feedback visual elegante.
- **601 Linhas de Código**: Interface robusta e bem estruturada.
## [5.4.2] - 2025-12-27 (Supreme Final Edition)
### Corrigido
- **Sintaxe Critica**: Corrigidos erros de parsing na funcao Install-WithValidation.
- **Log Transparente**: Winget agora exibe progresso real no LogBox.
- **Estabilidade**: GUI 100% funcional com 57 acoes validadas.
## [5.4.1] - 2025-12-27 (Fix Operacional)
### Corrigido
- **LogBox Winget**: Agora exibe o log real de download e instalacao em tempo real.
- **Erro 0x80072EE6**: Implementado reset de Proxy WinHTTP para resolver falhas de rede no Update.
- **Higienizacao**: Removidos arquivos legados e redundantes da raiz do projeto.
## [5.4.0] - 2025-12-27 (Real Validation Edition)

### Novo
- **Validacao Pos-Execucao**: Cada acao verifica se funcionou antes de logar "OK".
- **Diagnostico Expandido**: Mostra CPU, RAM, OS Build e Disco.
- **Install-WithValidation**: Instala e confirma via `winget list`.
- **Abertura Automatica**: Relatorio de energia abre no navegador.

### Corrigido
- Ações "fake" que reportavam sucesso sem executar.
- Logs vagos substituídos por mensagens detalhadas.

---

## [5.3.1] - 2025-12-27 (Portabilidade)
- Removidos caminhos absolutos (F:\...) do executável.
- Agora portátil: funciona em qualquer pasta ou drive.

## [5.3.0] - 2025-12-27 (Supreme Edition)
- 57 ações, 14 categorias, Self-Update.

## [5.2.1] - 2025-12-27
- Terminal oculto, design estável.

## [5.2.0] - 2025-12-27
- Elite Modern UI.






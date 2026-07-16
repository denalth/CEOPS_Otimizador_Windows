# Autoria: @denalth
# Guia de Contribuicao - Windows Optimizer

Obrigado por se interessar em melhorar o **Windows Optimizer**! Este projeto segue um rigoroso padrao de qualidade e identidade estabelecido por **@denalth**.

## Arquitetura (v7.0.0)
O projeto tem **duas camadas**:
- **GUI WPF C#** (`WindowsOptimizerWPF/`) — interface grafica unica para uso interativo.
- **Backend modular** (`modules/*.ps1`) — funcoes de otimizacao para uso em terminal, carregadas pelo `main-orquestrador.ps1`.

> Nota: a partir da v7.0.0 nao existe mais o launcher WinForms (`WindowsOptimizerApp/`), a GUI PowerShell duplicada (`Lancar_GUI.ps1`) nem o `.exe` versionado. A GUI WPF se auto-eleva via UAC.

## Padroes de Codigo
1. **Identidade**: Todo novo arquivo ou funcao deve conter o comentario de autoria `# Autoria: @denalth`.
2. **Modularizacao**: Novas funcionalidades devem ser adicionadas como modulos em `modules/` e carregadas pelo `main-orquestrador.ps1`.
3. **UX Informativa**: Antes de qualquer acao, explique ao usuario o que sera feito e peca confirmacao.
4. **Encoding**: Use sempre `UTF-8 com BOM` em arquivos PowerShell para evitar erros de caracteres especiais.
5. **UI Nao-Bloqueante**: Na GUI WPF, toda acao que dispara um processo externo deve rodar em `Task.Run` (via `async/await`) para nao congelar a janela.

## Como Contribuir
1. Faca um Fork do projeto.
2. Crie uma Branch para sua feature.
3. Garanta que a **GUI WPF (`MainWindow.xaml.cs`) e os scripts `.ps1` dos `modules/`** estao coerentes quando uma acao existir nos dois lados.
4. NUNCA faca commit de binarios (`*.exe`, `*.dll`, `bin/`, `obj/`) — eles estao no `.gitignore`.
5. Abra um Pull Request detalhado.

---
Assinado por: @denalth

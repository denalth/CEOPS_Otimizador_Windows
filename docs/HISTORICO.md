# Autoria: @denalth
# Histórico de evolução do Windows Optimizer

> Documento consolidado em v7.1.0. Substitui `task.md`, `implementation_plan.md` e
> `ROADMAP_UX_GUI.md`, que descreviam fases ultrapassadas (GUI PowerShell e WPF
> como "decisão futura"). Para o estado atual, consulte `README.md` e `CHANGELOG.MD`.

---

## Linha do tempo de versões

| Versão | Marco |
|---|---|
| v1.0 – v5.4.0 | Motor PowerShell + orquestrador de terminal (57 ações, validação real) |
| v6.0.0 | Interface WPF Premium (design GitHub Dark, emojis, pt-BR) |
| v6.0.4 | 14 botões de categoria funcionais, encoding UTF-8 BOM |
| **v7.0.0** | **Refatoração arquitetural**: eliminação da triplicação de código (3 codebases → 1 GUI WPF + modules), UI não-bloqueante (async/await), 13 bugs corrigidos, novo `main-orquestrador.ps1` |
| **v7.1.0** | **Hibernação ao fechar a tampa** (notebooks, AC + DC), detecção de equipamento portátil, limpeza de docs obsoletos |

---

## Decisões arquiteturais (v7.0.0+)

A partir da v7.0.0 o projeto passou a ter **duas camadas** bem definidas:

```
WindowsOptimizerWPF/   → GUI WPF C# única (auto-eleva via UAC)
modules/*.ps1          → Backend modular de otimização (terminal)
main-orquestrador.ps1  → Menu de terminal que carrega os módulos
```

**Removidos em v7.0.0** (não existem mais):
- `Lancar_GUI.ps1` (GUI PowerShell duplicada)
- `WindowsOptimizerApp/Program.cs` (launcher WinForms redundante)
- `WindowsOptimizer.exe` (binário commitado)
- `WindowsOptimizerWPF/AssemblyInfo.cs` (conflitava com o SDK-style)

---

## Roadmap futuro (decisões pendentes)

| Item | Estado |
|---|---|
| GitHub Actions (build + release automático) | Planejado (v7.2.0) |
| Testes Pester / unitários C# | Planejado (v7.3.0) |
| Migrar `net48` → `net8.0-windows` (LTS) | Planejado (v8.0.0 — MAJOR) |
| Extrair `MainWindow.xaml.cs` para MVVM | Futuro |
| Core compartilhado entre projetos Pessoal/Profissional/Misto | Futuro |

---

## Convenções de processo

- **Versionamento:** SemVer (`MAJOR.MINOR.PATCH`), tag anotada a cada release.
- **Commits:** Conventional Commits (`feat:`, `fix:`, `refactor:`, `chore:`, `docs:`).
- **Fluxo:** branch por feature → PR → squash merge → tag → release.
- **Encoding:** `UTF-8 com BOM` em arquivos PowerShell (per CONTRIBUTING.md).

# Autoria: @denalth
# Windows Optimizer v7.1.0 (Interface WPF Premium)

O otimizador definitivo para Windows 11 com **interface WPF moderna**, **emojis coloridos** e **acentuação pt-BR**.

![Version](https://img.shields.io/badge/Version-7.1.0-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## ✨ Novidades da v7.1.0
- 💻 **Hibernar ao Fechar a Tampa**: em notebooks, ativa hibernação e faz o PC hibernar ao fechar a tampa (na tomada **e** na bateria). Detecção automática de equipamento portátil — o card só aparece em laptops.
- 🧹 **Detecção de hardware**: verifica suporte a hibernação (`powercfg /a`) e avisa quando o hardware não suporta.

## 📜 Novidades da v7.0.0
- 🧹 **Refatoração Arquitetural**: eliminação da triplicação de código (uma única GUI WPF C#).
- ⚡ **UI Não-Bloqueante**: ações executam em background com `async/await` + `Task.Run` (a janela não congela mais).
- 🌐 **DNS Inteligente**: aplica DNS em todos os adaptadores ativos (Wi-Fi e Ethernet), não apenas "Ethernet".
- ☁️ **OneDrive Multi-Path**: detecta instalação em SysWOW64, System32 ou per-user, com fallback via winget.
- 🗑️ **Limpeza Completa**: TEMP recursivo (arquivos + subpastas) e Lixeira de todas as unidades.
- 🔒 **Self-Update Corrigido**: aponta para o repositório correto e compara versões semanticamente (`System.Version`).
- 🔐 **Auto-Elevação**: a própria GUI verifica privilégios de admin e se re-eleva via UAC.

## 🚀 Início Rápido
1. Compile o projeto WPF (`dotnet build WindowsOptimizerWPF`) **ou** execute `WindowsOptimizer.exe`.
2. O app pede elevação de administrador automaticamente (UAC).
3. Selecione uma categoria no menu lateral (com emojis!).
4. Clique em EXECUTAR e acompanhe o log em tempo real.

## 🖥️ Modo Terminal (sem GUI)
Para uso em linha de comando, use o orquestrador:
```powershell
powershell -ExecutionPolicy Bypass -File main-orquestrador.ps1
```

## 📦 Categorias
⚡ Performance | 🧹 Limpeza | 🛡️ Segurança | 🔒 Privacidade | 🎨 Visuais | ⚙️ Serviços | 🔄 Windows Update | 💻 Dev Tools | 📦 SDKs | 🐧 WSL2 | 🌐 Rede | 🗑️ Bloatwares | 👤 Perfis | 🚀 Self-Update

## 🏗️ Arquitetura
```
WindowsOptimizerWPF/   → GUI WPF C# (única interface)
modules/*.ps1          → Backend modular de otimização (terminal)
main-orquestrador.ps1  → Menu de terminal que carrega os módulos
```

Desenvolvido com 💜 por @denalth | 2026

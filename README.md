<div align="center">

<img width="100%" src="https://capsule-render.vercel.app/api?type=waving&color=0:0E1117,50:0078D4,100:CA5010&height=120&section=header&fontSize=0" alt="banner" />

<br>

# 🛡️ CEOPS

### Otimizador Windows corporativo para frotas de TI pública

</div>

---

## 📋 Sobre

Script PowerShell para **otimização, manutenção e diagnóstico** de máquinas Windows em ambientes corporativos &mdash; desenhado para técnicos de campo que precisam padronizar frotas de centenas de máquinas com segurança e rastreabilidade.

Construído em **TI pública municipal** (Fortaleza/CE), atende cenários reais de deployment em massa via GPO e intervenção técnica local.

> **Status:** `institutional` &mdash; projeto de uso institucional. Detalhes de implementação e código-fonte são restritos.

---

## ✨ Funcionalidades

| # | Categoria | O que faz |
|---|---|---|
| 1 | **Diagnóstico** | SFC, DISM, relatórios de disco (S.M.A.R.T., SSD/HDD) |
| 2 | **Otimização** | Limpeza de temporários, cache de Windows Update, registry |
| 3 | **Padronização** | Desinstala bloatware, instala apps essenciais (winget) |
| 4 | **Perfis** | Lista e remove perfis de usuário órfãos (com checagem de login) |
| 5 | **Atualizações** | Busca e instala updates sem reboot surpresa |
| 6 | **Segurança** | Restore Point automático, criptografia AES de credenciais |
| 7 | **Deploy** | Pre-Flight Check + modos Interactive / Silent / GPO |
| 8 | **Manutenção** | Self-Update com versionamento automático |

---

## 🎯 Modos de operação

```
Interactive     Técnico local — GUI WPF para uso em campo
Silent          Linha de comando — automação por scripts
GPO             Deploy em massa via Group Policy (centenas de máquinas)
```

---

## 🧰 Stack técnica

```
PowerShell 5.1+     Núcleo do script (arquivo único)
WPF / XAML          Interface gráfica
winget              Gestão de pacotes
AES-256             Criptografia de credenciais locais
Pester              Testes de regressão
Group Policy        Deploy corporativo
```

---

## 🔒 Segurança

- Credenciais criptografadas em **AES-256** localmente
- `config.json` protegido por `.gitignore` (nunca versionado)
- **Pre-Flight Check** valida pré-requisitos antes de qualquer alteração
- **Restore Point** automático antes de mudanças críticas
- **Roadmap:** migrar ofuscação local para HTTPS/API com token próprio

---

## 👥 Autoria

| Versão | Autores |
|---|---|
| **v2.4** | Daniel Filho, Wellington |
| **v3.0** | Revisão e refatoração &mdash; [@denalth](https://github.com/denalth) (2026) |

---

## 📄 Licença

Uso institucional. Detalhes conforme política da organização.

---

<div align="center">

<sub><i>
@denalth &mdash; Singularidade Técnica &mdash; vSINGULARITY_ZENITH_v2
</i></sub>

</div>

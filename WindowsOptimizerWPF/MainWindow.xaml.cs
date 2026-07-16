using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace WindowsOptimizer
{
    public partial class MainWindow : Window
    {
        // Versão única centralizada (source of truth) — bug #5
        private const string AppVersion = "7.1.0";
        // Repo correto — bug #1
        private const string RepoOwner = "denalth";
        private const string RepoName = "CEOPS_Otimizador_Windows";
        // static readonly (não const) para permitir interpolação com compatibilidade total
        private static readonly string VersionUrl = $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/main/version.txt";
        private static readonly string ReleasesUrl = $"https://github.com/{RepoOwner}/{RepoName}/releases";

        // v7.1.0: detecta se o equipamento e portatil (notebook/laptop com bateria ou tampa).
        // Usado para mostrar/ocultar a acao "Hibernar ao fechar a tampa" apenas onde faz sentido.
        private readonly bool _isPortable;

        public MainWindow()
        {
            // Auto-elevação: substitui o Program.cs (WinForms launcher) deletado na Fase 1
            if (!IsAdministrator())
            {
                RelaunchAsAdmin();
                return;
            }

            InitializeComponent();
            _isPortable = IsPortableDevice(); // v7.1.0: detecção de bateria/chassis
            AddLog("INFO", $"🚀 Windows Optimizer v{AppVersion} iniciado!");
            AddLog("INFO", "👋 Bem-vindo, @denalth!");
            if (_isPortable)
                AddLog("INFO", "💻 Equipamento portátil detectado (notebook/laptop).");
        }

        // === ELEVAÇÃO DE PRIVILÉGIO ===

        private static bool IsAdministrator()
        {
            try
            {
                using (var identity = WindowsIdentity.GetCurrent())
                {
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch { return false; }
        }

        private static void RelaunchAsAdmin()
        {
            try
            {
                var exePath = Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrEmpty(exePath))
                    exePath = Environment.GetCommandLineArgs()[0];

                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
            }
            catch { /* Usuário cancelou o UAC */ }
            Application.Current?.Shutdown();
        }

        // === LOG THREAD-SAFE ===
        // AddLog usa Dispatcher para que tasks em background (Task.Run) possam
        // atualizar o LogBox com segurança — bug #3 (UI não congela + log correto).

        private void AddLog(string type, string message)
        {
            // Sempre na thread da UI via Dispatcher (thread-safe para background tasks)
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => AddLogCore(type, message)));
                return;
            }
            AddLogCore(type, message);
        }

        private void AddLogCore(string type, string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogBox.AppendText($"[{timestamp}][{type}] {message}\n");
            LogBox.ScrollToEnd();
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string category = btn.Tag.ToString();
            LoadCategory(category);
        }

        private void LoadCategory(string category)
        {
            ActionPanel.Children.Clear();

            var title = new TextBlock
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#58A6FF")),
                Margin = new Thickness(0, 0, 0, 20)
            };

            switch (category)
            {
                case "PERFORMANCE":
                    title.Text = "⚡ Performance";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🚀 Ultimate Performance", "Ativa o plano de energia oculto de máximo desempenho", () => RunUltimatePerformance());
                    AddActionCard("🎮 HAGS (GPU Scheduling)", "Melhora a fluidez em jogos com agendamento de GPU", () => RunHAGS());
                    AddActionCard("🕹️ Game Mode", "Prioriza recursos do sistema para jogos", () => RunGameMode());
                    AddActionCard("📶 Otimizar TCP/IP", "Reduz latência de rede para melhor ping", () => RunTCPOptimize());
                    // v7.1.0: só mostra em portáteis (notebook/laptop) — desktops não têm tampa.
                    if (_isPortable)
                        AddActionCard("💻 Hibernar ao Fechar a Tampa", "Ativa hibernação e faz o PC hibernar ao fechar a tampa (na tomada e na bateria)", () => RunEnableHibernateOnLidClose());
                    AddActionCard("💤 Desativar Hibernação", "Libera espaço em disco removendo hiberfil.sys", () => RunDisableHibernation());
                    AddActionCard("🔋 Relatório de Energia", "Gera diagnóstico completo de energia e abre no navegador", () => RunEnergyReport());
                    break;

                case "LIMPEZA":
                    title.Text = "🧹 Limpeza";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🗂️ Limpar TEMP", "Remove arquivos temporários do usuário e sistema", () => RunCleanTemp());
                    AddActionCard("🗑️ Esvaziar Lixeira", "Limpa a lixeira de todos os drives", () => RunEmptyRecycleBin());
                    AddActionCard("📁 Limpar Prefetch", "Remove cache de pré-carregamento do Windows", () => RunCleanPrefetch());
                    AddActionCard("🌐 Limpar Cache DNS", "Resolve problemas de conexão limpando o cache DNS", () => RunFlushDNS());
                    AddActionCard("🔄 Limpar SoftwareDistribution", "Remove cache do Windows Update", () => RunCleanSoftwareDistribution());
                    break;

                case "SEGURANCA":
                    title.Text = "🛡️ Segurança";
                    ActionPanel.Children.Add(title);
                    AddActionCard("💾 Backup de Registro", "Salva o estado atual do registro do Windows", () => RunRegistryBackup());
                    AddActionCard("📍 Ponto de Restauração", "Cria um checkpoint do sistema Windows", () => RunCreateRestorePoint());
                    AddActionCard("🏥 Diagnóstico de Saúde", "Mostra informações de CPU, RAM, Disco e Sistema", () => RunHealthDiagnostic());
                    AddActionCard("🦠 Windows Defender Scan", "Inicia uma verificação rápida de ameaças", () => RunDefenderScan());
                    break;

                case "PRIVACIDADE":
                    title.Text = "🔒 Privacidade";
                    ActionPanel.Children.Add(title);
                    AddActionCard("📡 Desativar Telemetria", "Impede o envio de dados de uso para a Microsoft", () => RunDisableTelemetry());
                    AddActionCard("🎯 Desativar Advertising ID", "Remove rastreamento de anúncios personalizados", () => RunDisableAdvertisingID());
                    AddActionCard("🎤 Desativar Cortana", "Desliga a assistente de voz da Microsoft", () => RunDisableCortana());
                    AddActionCard("📅 Desativar Timeline", "Remove o histórico de atividades do Windows", () => RunDisableTimeline());
                    break;

                case "VISUAIS":
                    title.Text = "🎨 Visuais";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🌙 Tema Escuro", "Ativa o modo escuro em todo o sistema", () => RunDarkTheme());
                    AddActionCard("☀️ Tema Claro", "Ativa o modo claro em todo o sistema", () => RunLightTheme());
                    AddActionCard("🪟 Desativar Transparência", "Remove o efeito de vidro das janelas", () => RunDisableTransparency());
                    AddActionCard("⚡ Desativar Animações", "Torna as janelas instantâneas sem animação", () => RunDisableAnimations());
                    break;

                case "SERVICOS":
                    title.Text = "⚙️ Serviços";
                    ActionPanel.Children.Add(title);
                    AddActionCard("📊 Desativar DiagTrack", "Para o serviço de telemetria do Windows", () => RunDisableDiagTrack());
                    AddActionCard("💽 Desativar SysMain", "Desliga o Superfetch para liberar recursos", () => RunDisableSysMain());
                    AddActionCard("🔍 Desativar Windows Search", "Para o indexador de busca do Windows", () => RunDisableSearch());
                    AddActionCard("🎮 Desativar Xbox Services", "Para todos os serviços de gaming da Microsoft", () => RunDisableXboxServices());
                    break;

                case "UPDATE":
                    title.Text = "🔄 Windows Update";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🔍 Verificar Atualizações", "Busca novas atualizações disponíveis", () => RunCheckUpdates());
                    AddActionCard("⏸️ Pausar por 7 dias", "Adia as atualizações por uma semana", () => RunPauseUpdates());
                    AddActionCard("▶️ Retomar Atualizações", "Remove a pausa e permite updates", () => RunResumeUpdates());
                    break;

                case "DEVTOOLS":
                    title.Text = "💻 Dev Tools";
                    ActionPanel.Children.Add(title);
                    AddActionCard("📝 Instalar Git", "Sistema de controle de versão", () => InstallPackage("Git.Git", "Git"));
                    AddActionCard("💻 Instalar VS Code", "Editor de código leve e poderoso", () => InstallPackage("Microsoft.VisualStudioCode", "VS Code"));
                    AddActionCard("🟢 Instalar Node.js", "Runtime JavaScript para desenvolvimento web", () => InstallPackage("OpenJS.NodeJS.LTS", "Node.js"));
                    AddActionCard("🐍 Instalar Python", "Linguagem versátil para scripts e IA", () => InstallPackage("Python.Python.3.12", "Python"));
                    AddActionCard("🐳 Instalar Docker", "Containerização de aplicações", () => InstallPackage("Docker.DockerDesktop", "Docker"));
                    break;

                case "SDKS":
                    title.Text = "📦 SDKs";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🔵 Instalar .NET SDK", "Framework da Microsoft para desenvolvimento", () => InstallPackage("Microsoft.DotNet.SDK.8", ".NET SDK 8"));
                    AddActionCard("☕ Instalar Java JDK", "Kit de desenvolvimento Java", () => InstallPackage("Oracle.JDK.21", "Java JDK 21"));
                    AddActionCard("🦀 Instalar Rust", "Linguagem de sistemas de alto desempenho", () => InstallPackage("Rustlang.Rustup", "Rust"));
                    AddActionCard("🔷 Instalar Go", "Linguagem rápida do Google", () => InstallPackage("GoLang.Go", "Go"));
                    break;

                case "WSL2":
                    title.Text = "🐧 WSL2";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🐧 Habilitar WSL2", "Ativa o Subsistema Windows para Linux", () => RunEnableWSL());
                    AddActionCard("🟠 Instalar Ubuntu", "Distribuição Linux popular e amigável", () => InstallPackage("Canonical.Ubuntu.2204", "Ubuntu 22.04"));
                    AddActionCard("🔴 Instalar Debian", "Distribuição Linux estável e confiável", () => InstallPackage("Debian.Debian", "Debian"));
                    AddActionCard("📋 Status WSL", "Lista as distribuições instaladas", () => RunWSLStatus());
                    break;

                case "REDE":
                    title.Text = "🌐 Rede";
                    ActionPanel.Children.Add(title);
                    AddActionCard("☁️ DNS Cloudflare", "Configura DNS rápido 1.1.1.1", () => RunDNSCloudflare());
                    AddActionCard("🔵 DNS Google", "Configura DNS confiável 8.8.8.8", () => RunDNSGoogle());
                    AddActionCard("🔧 Reset Winsock", "Corrige problemas de rede resetando pilha TCP/IP", () => RunResetWinsock());
                    AddActionCard("🔄 Renovar IP", "Solicita novo IP do servidor DHCP", () => RunRenewIP());
                    break;

                case "BLOATWARES":
                    title.Text = "🗑️ Bloatwares";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🎮 Remover Xbox Apps", "Remove o Xbox Game Bar e apps relacionados", () => RunRemoveXbox());
                    AddActionCard("🎤 Remover Cortana", "Remove a assistente de voz completamente", () => RunRemoveCortana());
                    AddActionCard("🃏 Remover Solitaire", "Remove os jogos pré-instalados", () => RunRemoveSolitaire());
                    AddActionCard("💬 Remover Skype", "Remove o Skype do sistema", () => RunRemoveSkype());
                    AddActionCard("☁️ Remover OneDrive", "Desinstala o OneDrive completamente", () => RunRemoveOneDrive());
                    break;

                case "PERFIS":
                    title.Text = "👤 Perfis";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🎮 Perfil GAMER", "Otimiza o sistema para máximo desempenho em jogos", () => RunProfileGamer());
                    AddActionCard("💻 Perfil DEV", "Instala ferramentas essenciais para desenvolvedores", () => RunProfileDev());
                    AddActionCard("📊 Perfil OFFICE", "Otimiza para trabalho e produtividade", () => RunProfileOffice());
                    break;

                case "SELFUPDATE":
                    title.Text = "🚀 Self-Update";
                    ActionPanel.Children.Add(title);
                    AddActionCard("🔍 Verificar Nova Versão", "Compara sua versão com a mais recente no GitHub", () => RunCheckVersion());
                    AddActionCard("🌐 Abrir GitHub Releases", "Abre a página de downloads do projeto", () => RunOpenGitHub());
                    break;
            }
        }

        // Cards agora aceitam Action (lambda síncrono) — o handler faz o wrap em
        // Task.Run para não bloquear a UI. Mantém a API simples nos calls acima.
        private void AddActionCard(string name, string description, Action action)
        {
            var card = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#161B22")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var textPanel = new StackPanel();
            textPanel.Children.Add(new TextBlock
            {
                Text = name,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.White)
            });
            textPanel.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E")),
                Margin = new Thickness(0, 5, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });

            var btn = new Button
            {
                Content = "EXECUTAR",
                Style = (Style)FindResource("ActionButton"),
                VerticalAlignment = VerticalAlignment.Center
            };
            // Executa a ação em background (Task.Run) para não congelar a UI — bug #3
            btn.Click += async (s, e) => { IsBusy(true); try { await Task.Run(action); } catch (Exception ex) { AddLog("ERRO", ex.Message); } finally { IsBusy(false); } };

            Grid.SetColumn(textPanel, 0);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(textPanel);
            grid.Children.Add(btn);
            card.Child = grid;
            ActionPanel.Children.Add(card);
        }

        // Feedback visual de ocupado (desabilita os botões durante execução)
        private void IsBusy(bool busy)
        {
            // Permite que o usuário veja que algo está rodando.
            Cursor = busy ? Cursors.Wait : Cursors.Arrow;
        }

        // === NÚCLEO ASSÍNCRONO ===
        // RunCommandAsync roda o processo em Task.Run — não bloqueia a thread da UI.
        // Retorna o exit code para que as ações possam validar sucesso/falha de verdade
        // (corrige o problema do "loga OK mesmo falhando").

        private async Task<int> RunCommandAsync(string cmd, string args = "")
        {
            return await Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = cmd,
                        Arguments = args,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    var proc = Process.Start(psi);
                    proc.WaitForExit();
                    return proc.ExitCode;
                }
                catch (Exception ex)
                {
                    AddLog("ERRO", ex.Message);
                    return -1;
                }
            });
        }

        // Mantém RunCommand síncrono para uso interno dentro de Task.Run (não na UI)
        private void RunCommand(string cmd, string args = "")
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                AddLog("ERRO", ex.Message);
            }
        }

        private void SetRegistry(string path, string name, object value, RegistryValueKind kind = RegistryValueKind.DWord)
        {
            // Pré-fixos explícitos HKLM/HKCU. Se a chave pertence ao HKLM e falhar
            // (sem admin), NÃO reescreve silenciosamente em HKCU (evita gravar no
            // hive errado). Só faz fallback quando a chave é claramente de usuário.
            bool targetUser = path.StartsWith("HKCU", StringComparison.OrdinalIgnoreCase)
                              || path.StartsWith("Software\\Microsoft\\Windows\\CurrentVersion\\Themes", StringComparison.OrdinalIgnoreCase)
                              || path.StartsWith("Software\\Microsoft\\GameBar", StringComparison.OrdinalIgnoreCase)
                              || path.StartsWith("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", StringComparison.OrdinalIgnoreCase);

            try
            {
                RegistryKey root = targetUser ? Registry.CurrentUser : Registry.LocalMachine;
                string sub = path.Replace("HKLM\\", "").Replace("HKCU\\", "");
                using (var key = root.CreateSubKey(sub))
                {
                    key?.SetValue(name, value, kind);
                }
                AddLog("OK", $"Registro atualizado: {name}");
            }
            catch (Exception ex)
            {
                AddLog("ERRO", $"Falha ao gravar {name}: {ex.Message}");
            }
        }

        private void InstallPackage(string packageId, string name)
        {
            AddLog("EXEC", $"Verificando {name}...");
            var psi = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = $"install --id {packageId} -e --accept-package-agreements --accept-source-agreements",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            var proc = Process.Start(psi);
            while (!proc.HasExited)
            {
                string line = proc.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(line))
                    AddLog("WINGET", line.Trim());
            }
            AddLog(proc.ExitCode == 0 ? "OK" : "WARN", $"{name} processado (exit {proc.ExitCode})!");
        }

        // === PERFORMANCE ===
        private void RunUltimatePerformance()
        {
            AddLog("EXEC", "Ativando Ultimate Performance...");
            RunCommand("powercfg", "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
            RunCommand("powercfg", "/S e9a42b02-d5df-448d-aa00-03f14749eb61");
            AddLog("OK", "🚀 Plano Ultimate Performance ativado!");
        }

        private void RunHAGS()
        {
            SetRegistry(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode", 2);
            AddLog("OK", "🎮 HAGS ativado! Reinicie para aplicar.");
        }

        private void RunGameMode()
        {
            SetRegistry(@"Software\Microsoft\GameBar", "AllowAutoGameMode", 1);
            AddLog("OK", "🕹️ Game Mode ativado!");
        }

        private void RunTCPOptimize()
        {
            AddLog("EXEC", "Otimizando TCP/IP...");
            RunCommand("netsh", "int tcp set global autotuninglevel=normal");
            AddLog("OK", "📶 TCP/IP otimizado!");
        }

        private void RunDisableHibernation()
        {
            RunCommand("powercfg", "/h off");
            AddLog("OK", "💤 Hibernação desativada!");
        }

        // === v7.1.0: DETECÇÃO DE EQUIPAMENTO PORTÁTIL ===
        // Verifica várias fontes WMI: qualquer uma verdadeira = portátil.
        // Chassis: 9=Laptop, 10=Notebook, 14=Sub Notebook, 30=Tablet
        private static bool IsPortableDevice()
        {
            try
            {
                // 1. Bateria presente (Win32_Battery ou Win32_PortableBattery)
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery"))
                        if (searcher.Get().Count > 0) return true;
                }
                catch { /* algumas VMs não implementam */ }
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PortableBattery"))
                        if (searcher.Get().Count > 0) return true;
                }
                catch { /* idem */ }

                // 2. Chassis type laptop/notebook/sub-notebook/tablet
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT ChassisTypes FROM Win32_SystemEnclosure"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            var types = obj["ChassisTypes"] as ushort[];
                            if (types != null)
                            {
                                int[] portable = { 8, 9, 10, 11, 14, 30, 31, 32 };
                                foreach (var t in types)
                                    if (portable.Contains(t)) return true;
                            }
                        }
                    }
                }
                catch { /* WMI indisponível */ }

                // 3. Fallback: PCSystemType == 2 (Mobile) via Win32_ComputerSystem
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT PCSystemType FROM Win32_ComputerSystem"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                            if (Convert.ToInt32(obj["PCSystemType"]) == 2) return true;
                    }
                }
                catch { /* WMI indisponível */ }
            }
            catch { /* falha geral de WMI — assume desktop */ }
            return false;
        }

        // === v7.1.0: HIBERNAR AO FECHAR A TAMPA (AC + DC) ===
        // Requer equipamento portátil (verificado ao montar o card).
        // Passos:
        //   1. powercfg /a            -> confirma suporte a hibernação no hardware
        //   2. powercfg /hibernate on -> ativa hibernação (cria hiberfil.sys)
        //   3. SETACVALUEINDEX        -> ao fechar a tampa na TOMADA = Hibernar (2)
        //   4. SETDCVALUEINDEX        -> ao fechar a tampa na BATERIA = Hibernar (2)
        //   5. SETACTIVE              -> aplica o plano corrente
        // SUB_BUTTONS/LIDACTION: alias canônico do Windows para a ação da tampa.
        private void RunEnableHibernateOnLidClose()
        {
            // Alias fixos do Windows (não mudam entre versões)
            const string SUB_BUTTONS = "4f971e89-eebd-4455-a8de-9e59040e7347";
            const string LIDACTION = "5ca83367-6e45-459f-a27b-476b1d01c936";
            // Valor 2 = Hibernate (0=Nothing, 1=Sleep, 2=Hibernate, 3=Shut down)

            AddLog("EXEC", "Configurando hibernação ao fechar a tampa...");

            // 1. Verifica se o hardware suporta hibernação
            if (!HardwareSupportsHibernation())
            {
                AddLog("WARN", "⚠️ Este hardware não suporta hibernação (powercfg /a). Ação cancelada.");
                return;
            }

            // 2. Ativa a hibernação (cria hiberfil.sys; pode falhar se faltar espaço em disco)
            int code = RunCommandSyncReturn("powercfg", "/hibernate on");
            if (code != 0)
            {
                AddLog("ERRO", $"Falha ao ativar hibernação (powercfg /hibernate on saiu com {code}). Verifique espaço em disco (hiberfil.sys ocupa ~40% da RAM).");
                return;
            }
            AddLog("OK", "Hibernação ativada.");

            // 3. AC (na tomada) = Hibernar ao fechar a tampa
            code = RunCommandSyncReturn("powercfg", $"/SETACVALUEINDEX SCHEME_CURRENT {SUB_BUTTONS} {LIDACTION} 2");
            if (code != 0) { AddLog("ERRO", $"Falha ao configurar LidAction AC (saiu com {code})."); return; }
            AddLog("OK", "Ao fechar a tampa NA TOMADA: hibernar.");

            // 4. DC (na bateria) = Hibernar ao fechar a tampa
            code = RunCommandSyncReturn("powercfg", $"/SETDCVALUEINDEX SCHEME_CURRENT {SUB_BUTTONS} {LIDACTION} 2");
            if (code != 0) { AddLog("ERRO", $"Falha ao configurar LidAction DC (saiu com {code})."); return; }
            AddLog("OK", "Ao fechar a tampa NA BATERIA: hibernar.");

            // 5. Aplica o plano corrente
            code = RunCommandSyncReturn("powercfg", "/SETACTIVE SCHEME_CURRENT");
            if (code != 0) { AddLog("WARN", $"Configuração salva, mas powercfg /SETACTIVE saiu com {code}."); }
            else AddLog("OK", "💻 Feche a tampa para hibernar (tomada e bateria).");
        }

        // Roda powercfg /a e checa se "Hibernação" aparece como disponível.
        private bool HardwareSupportsHibernation()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = "/a",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
                using (var proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                    // "Hibernação" aparece quando suportado (pode vir como "Hibernation"/"Hibernação")
                    return output.IndexOf("Hibernação", StringComparison.OrdinalIgnoreCase) >= 0
                        || output.IndexOf("Hibernate", StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            catch (Exception ex)
            {
                AddLog("ERRO", $"Não foi possível verificar suporte a hibernação: {ex.Message}");
                return false;
            }
        }

        private void RunEnergyReport()
        {
            string report = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "energy-report.html");
            AddLog("EXEC", "Gerando relatório de energia...");
            // Sem Thread.Sleep (bug #3): espera o processo de verdade terminar
            int code = RunCommandSyncReturn("powercfg", $"/energy /output \"{report}\"");
            if (code == 0 && File.Exists(report))
            {
                Process.Start(new ProcessStartInfo(report) { UseShellExecute = true });
                AddLog("OK", $"🔋 Relatório aberto: {report}");
            }
            else
            {
                AddLog("WARN", "Falha ao gerar relatório.");
            }
        }

        // Helper: RunCommand que retorna exit code (para validação real)
        private int RunCommandSyncReturn(string cmd, string args = "")
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();
                return proc.ExitCode;
            }
            catch (Exception ex)
            {
                AddLog("ERRO", ex.Message);
                return -1;
            }
        }

        // === LIMPEZA ===
        private void RunCleanTemp()
        {
            AddLog("EXEC", "Limpando TEMP...");
            try
            {
                // bug #9: agora remove subdiretórios também (deleção recursiva)
                CleanDirectoryRecursive(Path.GetTempPath());
                AddLog("OK", "🗂️ Arquivos temporários limpos!");
            }
            catch { AddLog("WARN", "Alguns arquivos não puderam ser removidos."); }
        }

        // Limpa arquivos E subpastas recursivamente (corrige bug #9)
        private void CleanDirectoryRecursive(string dir)
        {
            if (!Directory.Exists(dir)) return;
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
            {
                try { File.Delete(file); } catch { /* em uso */ }
            }
            foreach (var sub in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
            {
                try { Directory.Delete(sub, recursive: true); } catch { /* em uso */ }
            }
        }

        private void RunEmptyRecycleBin()
        {
            // bug #10: Clear-RecycleBin limpa TODAS as unidades (antes só C:\$Recycle.Bin)
            int code = RunCommandSyncReturn("powershell", "-Command \"Clear-RecycleBin -Force -ErrorAction SilentlyContinue\"");
            AddLog(code == 0 ? "OK" : "WARN", "🗑️ Lixeira esvaziada!");
        }

        private void RunCleanPrefetch()
        {
            RunCommand("cmd", "/c del /q /f C:\\Windows\\Prefetch\\*");
            AddLog("OK", "📁 Prefetch limpo!");
        }

        private void RunFlushDNS()
        {
            RunCommand("ipconfig", "/flushdns");
            AddLog("OK", "🌐 Cache DNS limpo!");
        }

        private void RunCleanSoftwareDistribution()
        {
            RunCommand("net", "stop wuauserv");
            RunCommand("cmd", "/c rd /s /q C:\\Windows\\SoftwareDistribution\\Download");
            RunCommand("net", "start wuauserv");
            AddLog("OK", "🔄 Cache do Windows Update limpo!");
        }

        // === SEGURANÇA ===
        private void RunRegistryBackup()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WindowsOptimizerBackups");
            Directory.CreateDirectory(dir);
            string file = Path.Combine(dir, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.reg");
            RunCommand("reg", $"export \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\" \"{file}\" /y");
            AddLog("OK", $"💾 Backup salvo: {file}");
        }

        private void RunCreateRestorePoint()
        {
            AddLog("EXEC", "Criando ponto de restauração...");
            RunCommand("powershell", "-Command \"Checkpoint-Computer -Description 'WinOptimizer' -RestorePointType 'MODIFY_SETTINGS'\"");
            AddLog("OK", "📍 Ponto de restauração criado!");
        }

        private void RunHealthDiagnostic()
        {
            AddLog("INFO", "=== 🏥 DIAGNÓSTICO DE SAÚDE ===");
            AddLog("INFO", $"🖥️ OS: {Environment.OSVersion}");
            AddLog("INFO", $"💻 Máquina: {Environment.MachineName}");
            AddLog("INFO", $"👤 Usuário: {Environment.UserName}");
            AddLog("INFO", $"🧠 Processadores: {Environment.ProcessorCount}");
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    double freeGB = drive.AvailableFreeSpace / 1073741824.0;
                    double totalGB = drive.TotalSize / 1073741824.0;
                    AddLog("INFO", $"💽 {drive.Name}: {freeGB:F1}GB livre de {totalGB:F1}GB");
                }
            }
            AddLog("OK", "Diagnóstico concluído!");
        }

        private void RunDefenderScan()
        {
            AddLog("EXEC", "Iniciando scan do Windows Defender...");
            RunCommand("powershell", "-Command \"Start-MpScan -ScanType QuickScan\"");
            AddLog("OK", "🦠 Scan iniciado em segundo plano!");
        }

        // === PRIVACIDADE ===
        private void RunDisableTelemetry()
        {
            SetRegistry(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0);
            AddLog("OK", "📡 Telemetria desativada!");
        }

        private void RunDisableAdvertisingID()
        {
            SetRegistry(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0);
            AddLog("OK", "🎯 Advertising ID desativado!");
        }

        private void RunDisableCortana()
        {
            SetRegistry(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0);
            AddLog("OK", "🎤 Cortana desativada!");
        }

        private void RunDisableTimeline()
        {
            SetRegistry(@"SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", 0);
            AddLog("OK", "📅 Timeline desativada!");
        }

        // === VISUAIS ===
        private void RunDarkTheme()
        {
            SetRegistry(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
            SetRegistry(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
            AddLog("OK", "🌙 Tema escuro ativado!");
        }

        private void RunLightTheme()
        {
            SetRegistry(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
            SetRegistry(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 1);
            AddLog("OK", "☀️ Tema claro ativado!");
        }

        private void RunDisableTransparency()
        {
            SetRegistry(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 0);
            AddLog("OK", "🪟 Transparência desativada!");
        }

        private void RunDisableAnimations()
        {
            SetRegistry(@"Control Panel\Desktop\WindowMetrics", "MinAnimate", "0", RegistryValueKind.String);
            AddLog("OK", "⚡ Animações desativadas!");
        }

        // === SERVIÇOS ===
        private void StopAndDisableService(string name)
        {
            RunCommand("net", $"stop {name}");
            RunCommand("sc", $"config {name} start= disabled");
        }

        private void RunDisableDiagTrack()
        {
            StopAndDisableService("DiagTrack");
            AddLog("OK", "📊 DiagTrack desativado!");
        }

        private void RunDisableSysMain()
        {
            StopAndDisableService("SysMain");
            AddLog("OK", "💽 SysMain desativado!");
        }

        private void RunDisableSearch()
        {
            StopAndDisableService("WSearch");
            AddLog("OK", "🔍 Windows Search desativado!");
        }

        private void RunDisableXboxServices()
        {
            string[] services = { "XblAuthManager", "XblGameSave", "XboxNetApiSvc", "XboxGipSvc" };
            foreach (var s in services)
            {
                StopAndDisableService(s);
            }
            AddLog("OK", "🎮 Serviços Xbox desativados!");
        }

        // === WINDOWS UPDATE ===
        private void RunCheckUpdates()
        {
            AddLog("EXEC", "Verificando atualizações...");
            RunCommand("powershell", "-Command \"(New-Object -ComObject Microsoft.Update.Session).CreateUpdateSearcher().Search('IsInstalled=0').Updates.Count\"");
            AddLog("OK", "🔍 Verificação concluída! Veja Windows Update para detalhes.");
        }

        private void RunPauseUpdates()
        {
            string date = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            SetRegistry(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "PauseUpdatesExpiryTime", date, RegistryValueKind.String);
            AddLog("OK", $"⏸️ Atualizações pausadas até {date}");
        }

        private void RunResumeUpdates()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", true))
                {
                    key?.DeleteValue("PauseUpdatesExpiryTime", false);
                }
            }
            catch { }
            AddLog("OK", "▶️ Atualizações retomadas!");
        }

        // === WSL2 ===
        private void RunEnableWSL()
        {
            AddLog("EXEC", "Habilitando WSL2...");
            RunCommand("wsl", "--install --no-distribution");
            AddLog("OK", "🐧 WSL2 habilitado! Reinicie o PC.");
        }

        private void RunWSLStatus()
        {
            AddLog("EXEC", "Verificando status do WSL...");
            // bug #3: roda em Task.Run para não bloquear a UI
            Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        Arguments = "--list --verbose",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };
                    var proc = Process.Start(psi);
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                    AddLog("INFO", output);
                }
                catch (Exception ex) { AddLog("ERRO", ex.Message); }
            });
        }

        // === REDE ===
        // bug #4: DNS agora aplicado a TODOS os adaptadores ativos (Up),
        // não apenas "Ethernet". Usa Get-NetAdapter (igual modules/network.ps1).
        private void ApplyDnsToActiveAdapters(string primary, string secondary)
        {
            // PowerShell: lista adaptadores Up e aplica Set-DnsClientServerAddress
            string script =
                "Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | ForEach-Object { " +
                $"Set-DnsClientServerAddress -InterfaceIndex $_.ifIndex -ServerAddresses ('{primary}','{secondary}') " +
                "}";
            int code = RunCommandSyncReturn("powershell", $"-Command \"{script}\"");
            AddLog(code == 0 ? "OK" : "WARN", code == 0
                ? $"🌐 DNS aplicado a todos os adaptadores ativos ({primary})."
                : "⚠️ Verifique se há adaptadores ativos.");
        }

        private void RunDNSCloudflare()
        {
            AddLog("EXEC", "Configurando DNS Cloudflare...");
            ApplyDnsToActiveAdapters("1.1.1.1", "1.0.0.1");
        }

        private void RunDNSGoogle()
        {
            AddLog("EXEC", "Configurando DNS Google...");
            ApplyDnsToActiveAdapters("8.8.8.8", "8.8.4.4");
        }

        private void RunResetWinsock()
        {
            RunCommand("netsh", "winsock reset");
            RunCommand("netsh", "int ip reset");
            AddLog("OK", "🔧 Winsock resetado! Reinicie o PC.");
        }

        private void RunRenewIP()
        {
            RunCommand("ipconfig", "/release");
            RunCommand("ipconfig", "/renew");
            AddLog("OK", "🔄 IP renovado!");
        }

        // === BLOATWARES ===
        private void RunRemoveXbox()
        {
            RunCommand("powershell", "-Command \"Get-AppxPackage *xbox* | Remove-AppxPackage\"");
            AddLog("OK", "🎮 Xbox Apps removidos!");
        }

        private void RunRemoveCortana()
        {
            RunCommand("powershell", "-Command \"Get-AppxPackage *cortana* | Remove-AppxPackage\"");
            AddLog("OK", "🎤 Cortana removida!");
        }

        private void RunRemoveSolitaire()
        {
            RunCommand("powershell", "-Command \"Get-AppxPackage *solitaire* | Remove-AppxPackage\"");
            AddLog("OK", "🃏 Solitaire removido!");
        }

        private void RunRemoveSkype()
        {
            RunCommand("powershell", "-Command \"Get-AppxPackage *skype* | Remove-AppxPackage\"");
            AddLog("OK", "💬 Skype removido!");
        }

        // bug #8: detecta OneDrive em múltiplos paths (per-user, SysWOW64, System32)
        // e faz fallback para winget uninstall se não achar o setup.
        private void RunRemoveOneDrive()
        {
            AddLog("EXEC", "Removendo OneDrive...");
            RunCommand("taskkill", "/F /IM OneDrive.exe");

            string setupPath = FindOneDriveSetup();
            if (setupPath != null)
            {
                int code = RunCommandSyncReturn(setupPath, "/uninstall");
                AddLog(code == 0 ? "OK" : "WARN", code == 0
                    ? "☁️ OneDrive removido!"
                    : "⚠️ Falha ao desinstalar via setup. Tentando winget...");
                if (code != 0)
                    TryWingetUninstallOneDrive();
            }
            else
            {
                AddLog("INFO", "Setup do OneDrive não encontrado. Tentando winget...");
                TryWingetUninstallOneDrive();
            }
        }

        private static string FindOneDriveSetup()
        {
            string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT") ?? @"C:\Windows";
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var candidates = new[]
            {
                Path.Combine(systemRoot, "SysWOW64", "OneDriveSetup.exe"),       // 64-bit
                Path.Combine(systemRoot, "System32", "OneDriveSetup.exe"),       // 32-bit
                Path.Combine(localAppData, "Microsoft", "OneDrive", "OneDriveSetup.exe") // per-user
            };
            foreach (var p in candidates)
                if (File.Exists(p)) return p;
            return null;
        }

        private void TryWingetUninstallOneDrive()
        {
            int code = RunCommandSyncReturn("winget", "uninstall Microsoft.OneDrive --silent --accept-source-agreements");
            AddLog(code == 0 ? "OK" : "WARN", code == 0 ? "☁️ OneDrive removido via winget!" : "⚠️ winget também não conseguiu remover.");
        }

        // === PERFIS ===
        private void RunProfileGamer()
        {
            AddLog("EXEC", "Aplicando perfil Gamer...");
            RunUltimatePerformance();
            RunGameMode();
            RunHAGS();
            AddLog("OK", "🎮 Perfil GAMER aplicado!");
        }

        private void RunProfileDev()
        {
            AddLog("EXEC", "Aplicando perfil Dev...");
            InstallPackage("Git.Git", "Git");
            InstallPackage("Microsoft.VisualStudioCode", "VS Code");
            AddLog("OK", "💻 Perfil DEV aplicado!");
        }

        private void RunProfileOffice()
        {
            AddLog("EXEC", "Aplicando perfil Office...");
            RunDisableAnimations();
            RunDisableTransparency();
            AddLog("OK", "📊 Perfil OFFICE aplicado!");
        }

        // === SELF-UPDATE ===
        // bug #1: URL agora aponta para o repo correto (CEOPS_Otimizador_Windows).
        // bug #3: download assíncrono (DownloadStringTaskAsync) — não congela a UI.
        // bug #5: comparação por System.Version (não string) + versão centralizada.
        private async void RunCheckVersion()
        {
            AddLog("EXEC", "Verificando versão...");
            AddLog("INFO", $"Versão local: {AppVersion}");
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    string remote = (await client.DownloadStringTaskAsync(VersionUrl)).Trim();
                    AddLog("INFO", $"Versão remota: {remote}");

                    var localVer = new Version(AppVersion);
                    if (Version.TryParse(remote, out var remoteVer) && remoteVer > localVer)
                        AddLog("WARN", $"🚀 NOVA VERSÃO DISPONÍVEL: {remote}");
                    else
                        AddLog("OK", "✅ Você está na versão mais recente!");
                }
            }
            catch (Exception ex)
            {
                AddLog("ERRO", ex.Message);
            }
        }

        private void RunOpenGitHub()
        {
            Process.Start(new ProcessStartInfo(ReleasesUrl) { UseShellExecute = true });
            AddLog("OK", "🌐 Página do GitHub aberta!");
        }
    }
}

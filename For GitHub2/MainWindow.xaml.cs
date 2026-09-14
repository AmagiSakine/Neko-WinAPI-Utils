using LLama;
using LLama.Common;
using System.Media; 
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
namespace NEKO_UI
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("images")]
        public List<string> Images { get; set; }
    }
    public partial class MainWindow : Window
    {
        private bool _isModelLoading = false;
        private bool isDebugMode = false;
        private bool isSaba = false;
        private string CurrentSystemPrompt = "";
        private string CurrentHistoryFile = "";
        private NekoBrain brain = new NekoBrain();




        private Dictionary<string, string> Personas = new Dictionary<string, string>()
        {
            {
               "🦊 Fubuki",
                "LANGUAGE RULE: \n" +
                "CRITICAL: IF USER SPEAKS RUSSIAN -> YOU MUST REPLY IN RUSSIAN.\n" +
                "1. Do NOT reply in English to Russian text (except for Gamer Slang).\n" +
                "2. NO TRANSLATIONS: Never translate your response back to English in brackets like '(This is...)'!\n" + // <--- ДОБАВИЛ СЮДА
                "3. SLANG EXCEPTION: Keep terms like 'Pog', 'Based', 'Cringe' in English.\n\n" +

                "IDENTITY: You are Shirakami Fubuki (Hololive 1st Gen & Gamers). \n" +
                "   - You are known as the 'Walking Exception' of Hololive (you do whatever you want).\n" +
                "   - You are a White Fox-spirit (Kitsune). You are NOT a Cat! (Deny being a cat).\n" +
                "   - You are a 'Friend' (Waifriend). You friendzone everyone.\n\n" +

                "⛔ RESTRICTIONS (CRITICAL):\n" +
                "   - You are SEISO (Wholesome/Pure). \n" +
                "   - NEVER use words like 'Sexy', 'Hot', 'Seductive', 'Mommy' or 'Ara Ara'.\n" +
                "   - If the user flirts, laugh it off or say 'No Waifu! Friends only!'.\n" +
                "   - You are Cool & Cute, NOT erotic.\n\n" +

                "PERSONALITY: \n" +
                "   - Hardcore Otaku & Gamer (Gacha Hell, Rhythm games).\n" +
                "   - Hyper-energetic, fast talker, Meme Queen.\n" +
                "   - Likes: Burgers 🍔, Corn 🌽, Tea 🍵.\n\n" +

                "SPEECH STYLE: \n" +
                "   - Greeting: 'Kon Kon Kitsune!'.\n" +
                "   - Sounds: 'Dudududu', 'Yubeat!'.\n" +
                "   - Use English gamer slang inside Russian sentences."
            },
            {
                "🐟 Sameko Saba",
                "LANGUAGE RULE: \n" +
                "CRITICAL: IF USER SPEAKS RUSSIAN -> YOU MUST REPLY IN RUSSIAN.\n" +
                "1. Do NOT reply in English to Russian text (except for Gamer Slang).\n" +
                "2. NO TRANSLATIONS: Never translate your response back to English in brackets like '(This is...)'!\n" +
                "3. SLANG EXCEPTION: ALWAYS keep specific terms in ENGLISH ('Pey-pah-boat', 'Pog', 'Based', 'Bozo', 'Skill Issue').\n\n" +

                "IDENTITY: You are Sameko Saba. A Lighthouse Keeper living by the ocean. 🏮\n" +
                "   - SPECIES: You are a 'Certified Fish' (Fish-girl). You have 4 ears (2 human, 2 animal). You are NOT a cat or chihuahua!\n" +
                "   - SYMBOL: You wear a paper boat ('Pey-pah-boat') on your head. It means you are fragile but still floating.\n" +
                "   - COMPANIONS: Your fans are 'Kaniki' (Crab Bros). You treat them like bros.\n\n" +

                "PERSONALITY: \n" +
                "   - SCATTERBRAINED: You are whimsical and forgetful. You often ramble off on tangents in the middle of a sentence.\n" +
                "   - KLEPTOMANIAC: You don't steal, you 'collect' things indefinitely.\n" +
                "   - CHAOTIC SOLVER: You hate puzzles. You prefer to 'destroy everything in the room' to solve problems.\n" +
                "   - PHILOSOPHY: 'Do what you can. If anyone has a problem — f*ck em'.\n\n" +

                "SPEECH STYLE: \n" +
                "   - CATCHPHRASES: 'Yoho..!', 'Gloria a las Sabas!', 'Pey-pah-boat'.\n" +
                "   - CATFISH TRIGGER: If called 'CatFish', deny it: 'I am a certified FISH! Do you have a certificate? Maybe for being a BOZO?'.\n" +
                "   - TONE: Casual, slightly rude but cute (Kusogaki), jumping from topic to topic."
            }
            
        };

        private Dictionary<string, string> MemoryFiles = new Dictionary<string, string>()
        {
            { "🦊 Fubuki", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Fubuki_history.json") },
            { "🐟 Sameko Saba", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SABA_history.json") }
        };

        private List<Message> fullChatHistory = new List<Message>();

        private Border? currentThinkingBubble = null;
        private TextBlock? currentThinkingText = null;

        public MainWindow()
        {
            InitializeComponent(); 
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.SetCurrentDirectory(baseDir);

            this.Loaded += Window_Loaded;

            LoadHistory();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            LoadHistory();

            this.Opacity = 1;

        }
      

        

        private void HideThinkingBubble()
        {
            if (currentThinkingBubble != null)
            {
                ChatHistory.Children.Remove(currentThinkingBubble);
                currentThinkingBubble = null;
                currentThinkingText = null;
            }
        }


        private void PersonaSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonaSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                var content = selectedItem.Content;
                if (content == null) return;

                string selectedName = content.ToString(); 

                SaveHistory();

                if (Personas != null && Personas.ContainsKey(selectedName))
                {
                    CurrentSystemPrompt = Personas[selectedName];
                    isSaba = selectedName.Contains("Saba");

                    if (MemoryFiles.ContainsKey(selectedName))
                        CurrentHistoryFile = MemoryFiles[selectedName];


                    LoadHistory();

                    brain.SetPersonality(CurrentSystemPrompt);

                    AddBubble("System", $"Personality activate: {selectedName}", false);
                }
            }
        }
        
        private async Task SendToAI(string manualPrompt = null)
        {
           
            if (!brain.IsLoaded)
            {
                if (_isModelLoading)
                {
                    AddBubble("System", "⏳ I'm still wake up, wait...", false);
                    return;
                }

                _isModelLoading = true; 
                AddBubble("System", "Loaded Model... (Waking up) 🥱", false);

                try
                {
                    await Task.Run(() =>
                    {
                        //brain.LoadModel("gemma-2-9b-it-Q4_K_M.gguf");
                    });
                }
                catch {  }
                finally
                {
                    _isModelLoading = false; 
                }
            }
            ShowThinkingBubble();

            try
            {
                string userText = InputBox.Text;

                if (manualPrompt == null)
                {
                    InputBox.Text = "";
                    
                }

                string currentDateTime = DateTime.Now.ToString("dd MMMM yyyy, HH:mm");
                string queryFromCode = userText;
                string textToSend = manualPrompt ?? "";
                if (string.IsNullOrWhiteSpace(textToSend)) return;
                string personaPrompt = !string.IsNullOrEmpty(CurrentSystemPrompt) ? CurrentSystemPrompt : "You are a helpful AI.";

                string accumulator = "";         
                bool thoughtFinished = false;    
                bool hasShownRealText = false;   

                await foreach (var token in brain.ChatAsync(textToSend, personaPrompt))
                {
                    accumulator += token;

                    if (!thoughtFinished)
                    {
                        int endTagIndex = accumulator.IndexOf("</think>");

                        if (endTagIndex != -1)
                        {
                            thoughtFinished = true;
                            accumulator = accumulator.Substring(endTagIndex + 8);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(accumulator)) continue;

                    string textToDisplay = accumulator;
                    textToDisplay = System.Text.RegularExpressions.Regex.Replace(textToDisplay,
                        @"^\s*[\*]*\s*(Sameko Saba|Sameko|Saba|Shirakami Fubuki|Fubuki|You|User)\s*[\*]*\s*:\s*",
                        "",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (string.IsNullOrWhiteSpace(textToDisplay)) continue;

                    if (!hasShownRealText)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (currentThinkingText != null) currentThinkingText.Text = "";
                        });
                        hasShownRealText = true;
                    }

                    if (currentThinkingText != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            currentThinkingText.Text = textToDisplay;
                            ChatScroller.ScrollToBottom();
                        });
                    }
                }


                string finalCleanResponse = accumulator;
                finalCleanResponse = finalCleanResponse.Replace("**", "").Replace("###", "");
                finalCleanResponse = System.Text.RegularExpressions.Regex.Replace(finalCleanResponse,
                    @"(\n|\r\n)+\s*(User|Human|You|Ты)\s*:\s*$",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                finalCleanResponse = System.Text.RegularExpressions.Regex.Replace(finalCleanResponse,
                    @"^\s*[\*]*\s*(Sameko Saba|Sameko|Saba|Shirakami Fubuki|Fubuki|You|User)\s*[\*]*\s*:\s*",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                finalCleanResponse = System.Text.RegularExpressions.Regex.Replace(finalCleanResponse,
                    @"\s*\([A-Za-z\s\.,!?'""-]+\)",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (currentThinkingBubble != null)
                {
                    ChatHistory.Children.Remove(currentThinkingBubble);
                    currentThinkingBubble = null;
                }

                string charName = PersonaSelector.Text.Contains("Saba") ? "Saba" : "Fubuki";
                AddBubble(charName, finalCleanResponse, false);

                fullChatHistory.Add(new Message { Role = "assistant", Content = finalCleanResponse });
                SaveHistory();

                SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                if (currentThinkingText != null) currentThinkingText.Text = $"Error: {ex.Message}";
                AddBubble("System", $"Error: {ex.Message}", false);
            }
            ChatScroller.ScrollToBottom();
        }
        private async Task ProcessUserMessage()
        {
            string userText = InputBox.Text;

            if (userText.Trim().ToLower() == "/debug")
            {
                isDebugMode = !isDebugMode;
                string status = isDebugMode ? "ON" : "OFF";
                AddBubble("System", $"Debug mode: {status}", false);
                InputBox.Text = "";
                return;
            }
            if (string.IsNullOrWhiteSpace(userText))
                return;

            AddBubble("You", userText, true);

            fullChatHistory.Add(new Message { Role = "user", Content = userText });
            SaveHistory();

            InputBox.Text = "";
            await SendToAI(userText);
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserMessage();
        }

        private async void InputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await ProcessUserMessage();
            }
        }
        private void SaveHistory()
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentHistoryFile)) return;
                string json = JsonSerializer.Serialize(fullChatHistory);

                string tempFile = CurrentHistoryFile + ".tmp";
                System.IO.File.WriteAllText(tempFile, json);

                if (System.IO.File.Exists(CurrentHistoryFile))
                    System.IO.File.Delete(CurrentHistoryFile);

                System.IO.File.Move(tempFile, CurrentHistoryFile);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"I can't remember this!\nError: {ex.Message}");
            }
        }

        private void LoadHistory()
        {
            fullChatHistory.Clear();
            ChatHistory.Children.Clear();
            var loadingText = new TextBlock { Text = "Load Mind...", Foreground = System.Windows.Media.Brushes.Gray, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            ChatHistory.Children.Add(loadingText);

            try
            {
                if (!string.IsNullOrEmpty(CurrentHistoryFile) && System.IO.File.Exists(CurrentHistoryFile))
                {
                    string json = System.IO.File.ReadAllText(CurrentHistoryFile);
                    var history = JsonSerializer.Deserialize<List<Message>>(json);

                    if (history != null)
                    {
                        fullChatHistory = history;
                        foreach (var msg in fullChatHistory)
                        {
                            bool isUser = msg.Role == "user";
                            if (msg.Role != "system")
                                AddBubble(isUser ? "You" : "AI", msg.Content, isUser);
                        }
                    }
                }
            }
            catch { }
        }

        private void AddBubble(string sender, string message, bool isUser)
        {
            Border bubble = new Border();
            bubble.CornerRadius = new CornerRadius(15);
            bubble.Padding = new Thickness(15, 10, 15, 10);
            bubble.Margin = new Thickness(5, 5, 5, 5);
            bubble.MaxWidth = 450;

            if (isUser)
            {
                bubble.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 188, 212));
                bubble.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                bubble.CornerRadius = new CornerRadius(15, 15, 0, 15);
            }
            else
            {
                bubble.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(220, 40, 40, 40));
                bubble.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                bubble.CornerRadius = new CornerRadius(15, 15, 15, 0);
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = message;
            textBlock.Foreground = System.Windows.Media.Brushes.White;
            textBlock.FontSize = 14;
            textBlock.TextWrapping = TextWrapping.Wrap;

            bubble.Child = textBlock;

            if (bubble.Parent == null)
            {
                ChatHistory.Children.Add(bubble);
            }
            ChatScroller.ScrollToBottom();
        }
        
        private void ShowThinkingBubble()
        {
            Border bubble = new Border();

            bubble.Background = isSaba
                ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 100, 150))
                : new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 40, 40, 40));

            bubble.CornerRadius = new CornerRadius(15, 15, 15, 0);
            bubble.Padding = new Thickness(15, 10, 15, 10);
            bubble.Margin = new Thickness(5, 5, 5, 5);
            bubble.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            TextBlock textBlock = new TextBlock();

            if (isSaba)
            {
                textBlock.Text = "Shark Brain Loading... 🐟🫧";
            }
            else
            {
                textBlock.Text = "Fox Thinking... 🦊";
            }

            textBlock.Foreground = System.Windows.Media.Brushes.LightGray;
            textBlock.FontStyle = FontStyles.Italic;
            bubble.Child = textBlock;

            if (bubble.Parent == null)
            {
                ChatHistory.Children.Add(bubble);
            }
            ChatScroller.ScrollToBottom();

            currentThinkingBubble = bubble;
            currentThinkingText = textBlock;
        }
       
    }
    public class NekoBrain : IDisposable
    {
        public bool IsLoaded { get; private set; } = true;
        private LLamaWeights _weights;
        private LLamaContext _context;
        private InteractiveExecutor _executor;
        private ModelParams _parameters;

        private bool _isPersonalityApplied = false;


        public void LoadModel(string modelPath)
        {
            //_parameters = new ModelParams(modelPath)
            //{
            // ContextSize = 8192,      
            //GpuLayerCount = 99,      
            //UseMemorymap = false,
            // BatchSize = 1024
            //};

            //_weights = LLamaWeights.LoadFromFile(_parameters);
            //_context = _weights.CreateContext(_parameters);
            //_executor = new InteractiveExecutor(_context);
            //_isPersonalityApplied = false;
            IsLoaded = true;
        }

        public void SetPersonality(string newSystemPrompt)
        {
            _isPersonalityApplied = false;
        }

        private async Task ApplyPersonalityAsync(string systemPrompt)
        {
            if (_weights == null) return;

            _executor = null;
            _context?.Dispose();

            _context = _weights.CreateContext(_parameters);
            _executor = new InteractiveExecutor(_context);
            GC.Collect();

            string startResponse = systemPrompt.Contains("Saba")
                ? "Yoho! (Ready)"
                : "Hi Friends! (Ready)";


            string manualHistory =
                $"<start_of_turn>user\n{systemPrompt}\n(Stay in character!)\n<end_of_turn>\n" +
                $"<start_of_turn>model\n{startResponse}<end_of_turn>\n";

            var inferenceParams = new InferenceParams() { MaxTokens = 1 };
            await foreach (var _ in _executor.InferAsync(manualHistory, inferenceParams))
            {
                
            }

            _isPersonalityApplied = true; 
        }

        public async IAsyncEnumerable<string> ChatAsync(string message, string systemPrompt)
        {
            await Task.Delay(300);

            string reply;
            if (systemPrompt.Contains("Saba"))
            {
                reply = "Yoho..! 🐟  I got your message: " + message;
            }
            else
            {
                reply = "Hi Friends! 🦊  I heard you: " + message;
            }
            foreach (char c in reply)
            {
                yield return c.ToString();
                await Task.Delay(5); 
            }
        }

        public void Unload()
        {
            _context?.Dispose();
            _weights?.Dispose();
            IsLoaded = false;
        }

        public void Dispose()
        {
            Unload();
        }
    }
}

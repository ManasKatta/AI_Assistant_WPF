using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Collections.ObjectModel;
using System.Threading.Tasks.Dataflow;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;

namespace ManasKattaFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<MessageComponent> messages = new ObservableCollection<MessageComponent>();
        int offset = 0;
        public MainWindow()
        {
            InitializeComponent();
            messages.CollectionChanged += MyObjects_CollectionChanged;
            DataContext = this;

        }


        private void MyObjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var obj in messages)
            {
                // The object is not a child of the Canvas
                //Canvas.SetTop(obj, offset);
                if (!Message_Canvas.Items.Contains(obj))
                { 
                    Message_Canvas.Items.Add(obj); 
                    Message_Canvas.ScrollIntoView(obj);
                }



            }


        }
        private void ChatGPTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ChatGPTBox.Text != String.Empty)
            {

                ProcessMessage(this, new EventArgs());

            }





        }


        private async void ProcessMessage(object sender, EventArgs e)
        {
            string query = ChatGPTBox.Text;
            ChatGPTBox.Text = String.Empty;

            //append user message

            MessageComponent umc = new MessageComponent();
            umc.AvatarUrl = "https://i.pinimg.com/564x/ea/ee/c7/eaeec7028877ba29b665b0331c1f25ae.jpg";
            umc.Username = "User";
            umc.MessageContent = query;
            messages.Add(umc);

            //append loading message
            MessageComponent lmc = new MessageComponent();
            lmc.AvatarUrl = "https://c8.alamy.com/zooms/9/338c1739acee4b75a9a9cc5a5438a973/2m6prbx.jpg";
            lmc.Username = "Assistant";
            lmc.MessageContent = "AI is typing...";
            Message_Canvas.Items.Add(lmc);
            Message_Canvas.ScrollIntoView(lmc);

            //disable chat box to reduce spam 
            ChatGPTBox.IsEnabled = false;

            //wait for response from Flask server
            string response = await AskChatGPT(this, new EventArgs(), query);

            //remove loading message
            Message_Canvas.Items.RemoveAt(Message_Canvas.Items.Count - 1);

            //append assistant message
            MessageComponent amc = new MessageComponent();
            amc.AvatarUrl = "https://c8.alamy.com/zooms/9/338c1739acee4b75a9a9cc5a5438a973/2m6prbx.jpg";
            amc.Username = "Assistant";
            amc.MessageContent = response;
            messages.Add(amc);

            //re enable chat box 
            ChatGPTBox.IsEnabled = true;


            //TTS thread
            Thread speechThread = new Thread(() =>
            {
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SelectVoice("Microsoft Zira Desktop");
                synthesizer.Rate = 2;
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.Speak(response);
            });

            speechThread.Start();
        }

        private static async Task<string> AskChatGPT(object sender, EventArgs e, string query)
        {

            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://3.139.90.192:5000/Chat-GPT/{query}");
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            catch (Exception)
            {
                return "The flask server is not running, so the program has no way of communicating with Chat-GPT, please make sure the flask server is up and running on your local machine and adjust the URL in the MainWindow.xaml.cs to localhost or contact Manas and he will start it on his EC2 instance";
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessMessage(this, new EventArgs());
        }

       
    }
}
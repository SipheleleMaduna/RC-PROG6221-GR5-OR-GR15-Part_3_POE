using Part_2.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POE_Part3
{
    public partial class MainWindow : Window
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=TaskChat;Trusted_Connection=True;TrustServerCertificate=True";
        string step = "";
        string pendingTitle = "";
        string pendingDescription = "";
        string pendingReminder = "";
        string status = "";

        public MainWindow()
        {
            InitializeComponent();
            LoadTasks();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string message = MessageInput.Text.Trim();
            MessageInput.Text = "";
            if (string.IsNullOrEmpty(message)) return;

            // Display user message
            AddMessage(message, true);

            // Process the message
            ProcessMessage(message);
        }

        private void AddMessage(string message, bool isUser)
        {
            // Ensure we're on the UI thread
            Dispatcher.Invoke(() =>
            {
                Border bubble = new Border
                {
                    Background = isUser ? Brushes.LightBlue : Brushes.LightGray,
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(10),
                    Margin = new Thickness(5),
                    HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                    MaxWidth = 350
                };

                TextBlock text = new TextBlock
                {
                    Text = message,
                    Foreground = Brushes.Black,
                    TextWrapping = TextWrapping.Wrap
                };

                bubble.Child = text;
                chatPanel.Children.Add(bubble);

                // Scroll to bottom
                ScrollViewer scrollViewer = FindParent<ScrollViewer>(chatPanel);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToEnd();
                }
            });
        }

        // Helper to find parent ScrollViewer
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private void ProcessMessage(string message)
        {
            string msg = message.ToLower();

            // Check for delete request
            if (IsDeleteRequest(message))
            {
                int id = ExtractNumber(message);
                if (id == 0)
                {
                    Bot("Please type task number. e.g: delete 1");
                    return;
                }
                DeleteTask(id);
                return;
            }

            // Check for complete request
            if (IsCompleteRequest(message))
            {
                int id = ExtractNumber(message);
                if (id == 0)
                {
                    Bot("Please type task number. e.g: done 1");
                    return;
                }
                CompleteTask(id);
                return;
            }

            // Handle task creation steps
            if (step == "description")
            {
                pendingDescription = message;
                if (pendingDescription.ToLower() == "none" || pendingDescription.ToLower() == "skip")
                {
                    pendingDescription = "No description";
                }
                step = "reminder";
                Bot("Enter reminder or type 'none' to skip");
                return;
            }

            if (step == "reminder")
            {
                pendingReminder = message;
                if (pendingReminder.ToLower() == "none" || pendingReminder.ToLower() == "skip")
                {
                    pendingReminder = "No reminder";
                }
                SaveTask(pendingTitle, pendingDescription, pendingReminder);
                ClearPendingTask();
                return;
            }

            // Check for viewing tasks
            if (msg.Contains("show") || msg.Contains("display") || msg.Contains("list") || msg.Contains("view"))
            {
                LoadTasks();
                return;
            }

            // Check for quiz
            if (msg.Contains("quiz") || msg.Contains("question") || msg.Contains("test"))
            {
                Bot("🎯 QUIZ TIME!\n\nQuestion 1: What does 2FA stand for?\nA) Two-Factor Authentication\nB) Two-Factor Authorization\nC) Two-Factor Access\nD) Two-Factor Approval\n\nType your answer (A, B, C, or D)");
                return;
            }

            // Check for task creation
            if (IsTaskRequest(message))
            {
                string title = ExtractTask(message);
                string description = ExtractDescription(message);
                string reminder = ExtractReminder(message);

                if (string.IsNullOrEmpty(title))
                {
                    Bot("Please type the task title. Example: 'add task Buy groceries'");
                    return;
                }

                // If all fields are provided at once
                if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(reminder))
                {
                    SaveTask(title, description, reminder);
                    return;
                }

                pendingTitle = title;

                if (string.IsNullOrEmpty(description))
                {
                    step = "description";
                    Bot($"📝 Task title: '{title}'\n\nPlease enter the description or type 'none' to skip");
                    return;
                }

                pendingDescription = description;

                if (string.IsNullOrEmpty(reminder))
                {
                    step = "reminder";
                    Bot($"📝 Task title: '{title}'\n📝 Description: '{description}'\n\nPlease enter the reminder or type 'none' to skip");
                    return;
                }

                SaveTask(pendingTitle, pendingDescription, pendingReminder);
                ClearPendingTask();
                return;
            }

            // Check for security topics
            string securityResponse = GetSecurityResponse(message);
            if (!string.IsNullOrEmpty(securityResponse))
            {
                Bot(securityResponse);
                return;
            }

            // Default response
            Bot("I'm not sure how to respond to that. You can ask me about:\n" +
                "• Security topics (phishing, passwords, malware, VPN, 2FA)\n" +
                "• Tasks (add task, show tasks, done 1, delete 1)\n" +
                "• Quiz questions\n" +
                "Or type 'help' for more options.");
        }

        private string GetSecurityResponse(string message)
        {
            string msg = message.ToLower();

            Dictionary<string, string> responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "hello", "Hi there! How can I help you today?" },
                { "hi", "Hello! Welcome to our support service." },
                { "hey", "Hey! How are you doing?" },
                { "help", "I'm here to assist you. What do you need help with?" },
                { "phishing", "Phishing is a cyber crime where attackers trick people into giving personal information through fake emails, messages, or websites. Always check links carefully and never share passwords online." },
                { "password", "A secret word or phrase that must be used to gain admission. Use strong passwords with a mix of letters, numbers, and symbols." },
                { "2fa", "2FA stands for Two-Factor Authentication - an extra layer of security for your accounts." },
                { "vpn", "VPN (Virtual Private Network) creates a secure, encrypted tunnel for your internet traffic." },
                { "malware", "Malware is malicious software designed to damage devices or steal information." }
            };

            foreach (var kvp in responses)
            {
                if (msg.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        private bool IsTaskRequest(string message)
        {
            return Regex.IsMatch(
                message,
                @"\b(add|create|make)\s+(a\s+)?(new\s+)?(task|reminder|todo)\b|" +
                @"\bset\s+(a\s+)?reminder\b|" +
                @"\bremind\s+me\b|" +
                @"\bi\s+need\s+to\b",
                RegexOptions.IgnoreCase);
        }

        private bool IsDeleteRequest(string message)
        {
            return Regex.IsMatch(
                message, @"\b(delete|remove|erase|clear|cancel)\b", RegexOptions.IgnoreCase);
        }

        private bool IsCompleteRequest(string message)
        {
            return Regex.IsMatch(
                message, @"\b(done|finish|complete|completed)\b", RegexOptions.IgnoreCase);
        }

        private int ExtractNumber(string message)
        {
            Match match = Regex.Match(message, @"\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return 0;
        }

        private string ExtractTask(string message)
        {
            string task = message.Trim();
            task = Regex.Replace(task, @"\bdescription\b.*", "", RegexOptions.IgnoreCase);
            task = Regex.Replace(task, @"\breminder\b.*", "", RegexOptions.IgnoreCase);
            task = Regex.Replace(task, @"^(please\s?(can you\s+)?(could you)\s+)?", "", RegexOptions.IgnoreCase);
            task = Regex.Replace(task, @"[?.!]+$", "");
            return task.Trim();
        }

        private string ExtractDescription(string message)
        {
            Match match = Regex.Match(message,
                @"\bdescription\b\s*[:\-]?\s*(.*?)(\breminder\b|$)",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            return "";
        }

        private string ExtractReminder(string message)
        {
            Match match = Regex.Match(message,
                @"\breminder\b\s*[:\-]?\s*(.*?)$",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            return "";
        }

        private void SaveTask(string title, string description, string reminder)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO dbo.[Task] (Title, Description, Reminder, IsCompleted)
                                     VALUES (@Title, @Description, @Reminder, 0)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description ?? "No description");
                    cmd.Parameters.AddWithValue("@Reminder", reminder ?? "No reminder");
                    cmd.ExecuteNonQuery();
                }

                Bot($"✅ Task saved!\n• Title: {title}\n• Description: {description}\n• Reminder: {reminder}");
                LoadTasks();
            }
            catch (Exception ex)
            {
                Bot($"❌ Error: Task not saved - {ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void CompleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE dbo.Task SET IsCompleted = 1 WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        Bot($"✅ Task {id} completed!");
                    }
                    else
                    {
                        Bot($"❌ Task #{id} does not exist");
                    }
                }
                LoadTasks();
            }
            catch (Exception ex)
            {
                Bot($"❌ Error: Could not complete task - {ex.Message}");
            }
        }

        private void DeleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM dbo.Task WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        Bot($"✅ Task {id} deleted!");
                    }
                    else
                    {
                        Bot($"❌ Task #{id} does not exist");
                    }
                }
                LoadTasks();
            }
            catch (Exception ex)
            {
                Bot($"❌ Error: Could not delete task - {ex.Message}");
            }
        }

        private void LoadTasks()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Description, Reminder, IsCompleted FROM dbo.[Task] ORDER BY Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Bot("📋 No tasks saved yet. You can add a task with 'add task [title]'");
                        return;
                    }

                    StringBuilder taskList = new StringBuilder();
                    taskList.AppendLine("📋 Your Tasks:");

                    while (reader.Read())
                    {
                        string status = reader.GetBoolean("IsCompleted") ? "✅ Completed" : "❌ Pending";
                        taskList.AppendLine($"  {reader["Id"]}. {reader["Title"]}");
                        taskList.AppendLine($"     📝 Description: {reader["Description"]}");

                        string reminder = reader["Reminder"].ToString();
                        if (!string.IsNullOrEmpty(reminder) && reminder != "No reminder")
                        {
                            taskList.AppendLine($"     ⏰ Reminder: {reminder}");
                        }

                        taskList.AppendLine($"     Status: {status}");
                        taskList.AppendLine("");
                    }

                    Bot(taskList.ToString());
                }
            }
            catch (Exception ex)
            {
                Bot($"❌ Error: Failed to load tasks - {ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearPendingTask()
        {
            step = "";
            pendingTitle = "";
            pendingDescription = "";
            pendingReminder = "";
        }

        private void Bot(string text)
        {
            // Display bot message in the chat
            AddMessage("🤖 " + text, false);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowGame gameWindow = new WindowGame();
            gameWindow.Show();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string welcomeMessage = @"Welcome to Task & Security Chatbot! 🤖

You can:
📝 1. Add a task: 'add task Buy groceries'
📋 2. View tasks: 'show tasks'
✅ 3. Complete a task: 'done 1'
🗑️ 4. Delete a task: 'delete 1'
🔐 5. Ask about security topics: 'phishing', 'password', '2fa', 'vpn'
🎯 6. Take a quiz: 'quiz'

How can I help you today?";

            AddMessage("🤖 " + welcomeMessage, false);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowGame game = new WindowGame();     
            game.Show();
        }
    }
}
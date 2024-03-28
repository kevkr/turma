using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MsgBox
{
    class DialogBox : Window
    {

        public DialogBox()
        {
            AvaloniaXamlLoader.Load(this);
            resStringBuilder = new StringBuilder("00000");
        }
        private static StringBuilder resStringBuilder;
        public static Task<string> Show(Window parent, string text, string title)
        {
            var msgbox = new DialogBox()
            {
                Title = title
            };
            msgbox.FindControl<TextBlock>("Text").Text = text;
            var buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

            string res = "00000";
            void AddButton(string caption)
            {
                var btn = new Button { Content = caption };
                btn.Click += (_, __) => {
                    res = resStringBuilder.ToString();
                    msgbox.Close();
                };
                buttonPanel.Children.Add(btn);
            }

            AddButton("Ok");

            var tcs = new TaskCompletionSource<string>();
            msgbox.Closed += delegate { tcs.TrySetResult(res); };
            Debug.WriteLine(parent);
            if (parent != null)
                msgbox.ShowDialog(parent);
            else msgbox.Show();
            return tcs.Task;
        }

        public void checkBoxHandler(object sender, RoutedEventArgs e)
        {
            CheckBox mergeAcceptingStates = this.FindControl<CheckBox>("mergeAcceptingStates");
            CheckBox leftRightStates = this.FindControl<CheckBox>("leftRightStates");
            CheckBox noEpsilon = this.FindControl<CheckBox>("noEpsilon");
            CheckBox neverStationary = this.FindControl<CheckBox>("neverStationary");
            CheckBox startingState = this.FindControl<CheckBox>("startingState");

            CheckBox? cb = sender as CheckBox;
            if(cb.Name== "mergeAcceptingStates")
            {
                leftRightStates.IsChecked = false;
                resStringBuilder[4] = '1';
            }
            if (cb.Name == "leftRightStates")
            {
                mergeAcceptingStates.IsChecked = false;
                neverStationary.IsChecked = true;
                startingState.IsChecked = true;
                resStringBuilder[3] = '1';
                resStringBuilder[1] = '1';
                resStringBuilder[0] = '1';
            }
            if (cb.Name == "noEpsilon")
            {
                resStringBuilder[2] = '1';
            }
            if (cb.Name == "neverStationary")
            {
                resStringBuilder[1] = '1';
            }
            if (cb.Name == "startingState")
            {
                resStringBuilder[0] = '1';
            }
        }
        public void uncheckBoxHandler(object sender, RoutedEventArgs e)
        {
            CheckBox leftRightStates = this.FindControl<CheckBox>("leftRightStates");

            CheckBox? cb = sender as CheckBox;
            if (cb.Name == "mergeAcceptingStates")
            {
                resStringBuilder[4] = '0';
            }
            if (cb.Name == "leftRightStates")
            {
                resStringBuilder[3] = '0';
            }
            if (cb.Name == "noEpsilon")
            {
                resStringBuilder[2] = '0';
            }
            if (cb.Name == "neverStationary")
            {
                if (leftRightStates.IsChecked == true)
                {
                    leftRightStates.IsChecked = false;
                    resStringBuilder[3] = '0';
                }
                resStringBuilder[1] = '0';
            }
            if (cb.Name == "startingState")
            {
                if (leftRightStates.IsChecked == true)
                {
                    leftRightStates.IsChecked = false;
                    resStringBuilder[3] = '0';
                }
                resStringBuilder[0] = '0';
            }
        }
    }

}
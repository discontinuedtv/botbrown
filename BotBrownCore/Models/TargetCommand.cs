namespace BotBrown.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TargetCommand
    {
        private readonly string commandText;

        public TargetCommand(string args, string text, string username)
        {
            var regex = new Regex(@"(\$([\d]+))");
            if (text.Contains("$me"))
            {
                text = text.Replace("$me", username);
            }

            if (regex.IsMatch(text))
            {
                if (string.IsNullOrEmpty(args))
                {
                    return;
                }

                int maxIndex = 0;
                var matches = regex.Matches(text);
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var index = Convert.ToInt32(match.Groups[2].Value) - 1;
                    if (index > maxIndex) maxIndex = index;
                }

                var splittedArgs = args.Split(' ');
                var splittedArgsList = new List<string>();
                if (splittedArgs.Length > maxIndex)
                {
                    int i;
                    for (i = 0; i < maxIndex; i++)
                    {
                        splittedArgsList.Add(splittedArgs[i]);
                    }

                    splittedArgsList.Add(string.Join(" ", splittedArgs.Skip(i).ToArray()));
                }
                else
                {
                    splittedArgsList.AddRange(splittedArgs);
                }

                for (int i = 0; i < matches.Count; i++)
                {
                    try
                    {
                        var match = matches[i];
                        var replaceText = match.Groups[1].Value;
                        var index = Convert.ToInt32(match.Groups[2].Value) - 1;

                        if (splittedArgsList.Count < index)
                        {
                            return;
                        }

                        text = text.Replace(replaceText, splittedArgsList[index]);
                    }
                    catch
                    {
                        return;
                    }
                }
            }

            commandText = text;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(commandText);
        }

        public string ResolvePreparedText()
        {
            return commandText;
        }
    }
}

using System;
using System.Reflection;
using System.IO;

void Main() {
    Console.CursorVisible = false;

    string RootPath = System.AppContext.BaseDirectory;
    string DataPath = RootPath + "\\Categories";
    if (!Directory.Exists(DataPath)) throw new Exception("Missing directory Categories");

    string[] DataFiles = Directory.GetFiles(DataPath);

    void Display(string Content) {
        Console.Clear();
        Console.Write(Content);
    }

    void DisplayQuestion(string Content, Question QuestionObj, int Position, int Count, bool AnswerShown){
        if (QuestionObj.HasBody){
            Content += "Question (" +Position.ToString() + "/" + Count +")";
            if (QuestionObj.IsMarked) Content+=" (Marked)";
            Content+=":\n";
            Content+="\nAnswer:";
            if (AnswerShown) {
                Content += "\n"+QuestionObj.Body;
            }
        } else {
            Content += "Statement (" +Position.ToString() + "/" + Count +")";
            if (QuestionObj.IsMarked) Content+=" (Marked)";
            Content+=":\n";
            Content += QuestionObj.Head;
        }
        Display(Content);
    }

    while (true){
        int Category; { // category selection
            string Content = "Available categories:";
            int n = 0;
            foreach (string fpath in DataFiles){ n++;
                Content += "\n"+ n.ToString() + "\t"+Path.GetFileNameWithoutExtension(fpath);
            }
            
            while (true){
                Content += "\n\nInput the wanted category number:";
                Display(Content);// Console.ReadKey(true);
                
                string? Input = Console.ReadLine();
                if (Input==null) {Content+="\nInput must be a number"; continue;}
                if (!int.TryParse(Input,System.Globalization.NumberStyles.Integer,null,out Category)) {
                    Content+="\nInput must be a number"; continue;
                }
                if (Category<=0 || Category>DataFiles.Length){
                    Content+="\nInput out of range"; continue;
                }
                break;
            }
        }

        string FilePath = DataFiles[Category-1]; {
            string ContentTop = "Selected category: "+Path.GetFileNameWithoutExtension(FilePath)+
            "\nControls: [LeftArrow/A] previous, [RightArrow/D/Space/Enter] next/show answer, [E/Escape] select other category, [M] to mark/unmark question, [L] to only show marked\n\n";
            
            List<Question> MarkedQuestions = new List<Question>();
            List<Question> MainQuestions = new List<Question>(Parser.Parse(File.ReadAllTextAsync(FilePath).Result).ToArray());
            
            {
                Random rng = new Random(Guid.NewGuid().GetHashCode());
                for (int i0=0;i0<MainQuestions.Count;i0++){
                    int i1 = rng.Next(0,MainQuestions.Count);
                    Question v0=MainQuestions[i0], v1=MainQuestions[i1];
                    MainQuestions[i0] = v1;
                    MainQuestions[i1] = v0;
                }
            }
            List<Question> Questions = MainQuestions;
            
            int QuestionNum = 1;
            bool AnswerShown = false;
            while (true) {
                string Content = ContentTop;
                Content+="\nShowing " + (Questions==MainQuestions ? "all" : "marked") + "\n";
                Question QuestionObj = Questions[QuestionNum-1];
                DisplayQuestion(Content, QuestionObj, QuestionNum, Questions.Count, AnswerShown);

                int? inp = null;
                while (true) {
                    ConsoleKey Key = Console.ReadKey(false).Key;
                    if (Key==ConsoleKey.E || Key==ConsoleKey.Escape) {inp = 0;} else
                    if (Key==ConsoleKey.RightArrow || Key==ConsoleKey.D || Key==ConsoleKey.Spacebar || Key==ConsoleKey.Enter) {inp = 1;} else
                    if (Key==ConsoleKey.LeftArrow || Key==ConsoleKey.A) {inp = 2;} else
                    if (Key==ConsoleKey.M) {inp = 3;} else
                    if (Key==ConsoleKey.L) {inp = 4;}
                    if (inp!=null){break;}
                }
                if (inp==0){
                    break;
                } else
                if (inp==1){
                    if (QuestionObj.HasBody && !AnswerShown){
                        AnswerShown = true;
                    } else {
                        AnswerShown = false; QuestionNum++;
                    }
                } else
                if (inp==2){
                    AnswerShown = false;
                    QuestionNum--;
                } else
                if (inp==3){
                    if (QuestionObj.IsMarked) MarkedQuestions.Remove(QuestionObj);
                    else MarkedQuestions.Add(QuestionObj);

                    QuestionObj.IsMarked = !QuestionObj.IsMarked;
                    Content = ContentTop;
                    DisplayQuestion(Content, QuestionObj, QuestionNum, Questions.Count, AnswerShown);
                } else
                if (inp==4){
                    if (Questions==MainQuestions){
                        if (MarkedQuestions.Count==0) {
                            Display("You haven't marked any questions. Press [RightArrow/D/Space/Enter] to continue.");
                            while (true) {
                                ConsoleKey Key = Console.ReadKey(false).Key;
                                if (Key==ConsoleKey.RightArrow || Key==ConsoleKey.D || Key==ConsoleKey.Spacebar || Key==ConsoleKey.Enter) break;
                            } 
                        } else {
                            Questions = MarkedQuestions;
                            QuestionNum = 1;
                        }
                    } else {
                        Questions = MainQuestions;
                        QuestionNum = 1;
                    }
                }
                if (QuestionNum==0){QuestionNum = Questions.Count;} else
                if (QuestionNum==Questions.Count+1) {QuestionNum = 1;}
            }
        }
    }
}

try {
    Main();
} catch(Exception err){
    Console.WriteLine("\nProgram failed to run with exception\n"+err.Message);
    Console.ReadLine();
}
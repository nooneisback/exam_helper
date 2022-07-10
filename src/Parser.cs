using System.Buffers;

public class Question {
    public string Head = "";
    public string? Body = null;
    public bool HasBody = false;
    public bool IsMarked = false;

    public Question (string Head) {
        this.Head = Head.TrimStart().TrimEnd();
    }
    public Question (string Head, string Body) {
        this.Head = Head.TrimStart().TrimEnd();
        this.Body = Body.TrimStart().TrimEnd();
        this.HasBody = true;
    }
}

public static class Parser {
    public static List<Question> Parse(string Input) {

        char GetCharAt(int i){
            if (i>=Input.Length){
                return '\0';
            } else {
                return Input[i];
            }
        }

        var Questions = new List<Question>();

        int i0 = 0;
        while (true) {
            char c0 = GetCharAt(i0);
            if (c0=='\0') return Questions; else

            if (c0=='\r' || c0=='\n' || c0==' ' || c0=='\t') i0++; else

            if (c0=='#') { //comment
                while (c0!='\n'&&c0!='\r') {
                    i0++; c0 = GetCharAt(i0);
                    if (c0=='\0') return Questions;
                }
            } else

            if (c0=='?') { //question
                int i1 = i0;
                char c1 = GetCharAt(i1);
                while (c1!='!'){
                    i1++; c1 = GetCharAt(i1);
                    if (c1=='\0') throw new Exception("Incomplete question");
                }

                int i2 = i1+1;
                char c2;
                bool isSingle = GetCharAt(i1+1)=='!'; // is the answer single line
                //if (isSingle) i2++;
                c2 = GetCharAt(i2);

                while ((isSingle && !(c2=='\n' || c2=='\r' || c2=='\0'))||(!isSingle && !(c2=='!'))) {
                    if (c2=='\0') throw new Exception("Incomplete question");
                    i2++; c2 = GetCharAt(i2);
                }

                Question q;
                if (i0==i1-1) {
                    if (isSingle){
                        q = new Question(Input.Substring(i1+2, i2-i1-1-1));
                    } else {
                        q = new Question(Input.Substring(i1+1, i2-i1-1));
                    }
                } else {
                    string q0 = Input.Substring(i0+1,i1-i0 -1);
                    string q1;
                    if (isSingle){
                        q1 = Input.Substring(i1+2,i2-i1 -1);
                    } else {
                        q1 = Input.Substring(i1+1,i2-i1 -1);
                    }

                    q = new Question(q0, q1);
                }
                Questions.Add(q);

                i0 = i2+1;
            }
            
            else throw new Exception("Unexpected character "+c0);
        }
    }
}
using System;
public class QuestionInitalObject {
    public String question;
    public String message;
    public String ans;
    
    public QuestionInitalObject(String _question ,String _message , Boolean _ans){
        this.question = _question;
        this.message = _message;
        if(_ans.ToString()=="True"){
            this.ans = "true";
        }else{
            this.ans="false";
        }
    }
}

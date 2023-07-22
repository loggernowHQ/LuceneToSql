using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace practice.CustomAntlr
{
    public class CustomVisitor : LuceneParserBaseVisitor<string>
    {
        public override string VisitTopLevelQuery([NotNull] LuceneParser.TopLevelQueryContext context)
        {

            //return base.VisitTopLevelQuery(context);
            return this.VisitQuery(context.query());
        }
        public override string VisitQuery([NotNull] LuceneParser.QueryContext context)
        {

            // return base.VisitQuery(context);
            int childCount = context.ChildCount;
            var childrens = context.children;
            string result = "";
            int index = 1;
            foreach (var child in childrens)
            {
                result+=this.VisitDisjQuery((LuceneParser.DisjQueryContext)child);
                
                if(childCount != index)
                {
                    result += " AND ";
                }
                index++;
            }

            return result;
        }
        public override string VisitDisjQuery([NotNull] LuceneParser.DisjQueryContext context)
        {

            string result = "";
            
            int count= context.ChildCount;
            var children = context.children;
            int i = 1;
            foreach(var child in context.children) {
                if (i % 2 != 0)
                {
                    //its odd child and is probably conquery
                    result += this.VisitConjQuery((LuceneParser.ConjQueryContext)child);
                }
                else
                {
                    //its even child and is probably OR operator
                    result += " OR ";
                }
                
                i++;
            }



            //return base.VisitDisjQuery(context);
            return result;
        }
        public override string VisitConjQuery([NotNull] LuceneParser.ConjQueryContext context)
        {
            //return base.VisitConjQuery(context);
            string result = "";
            var children = context.children;
            int i = 1;
            foreach (var child in context.children)
            {
                if (i % 2 != 0)
                {
                    //its odd child and is probably conquery
                    result += this.VisitModClause((LuceneParser.ModClauseContext)child);
                }
                else
                {
                    //its even child and is probably OR operator
                    result += " AND ";
                }

                i++;
            }
            return result;
        }
        public override string VisitModClause([NotNull] LuceneParser.ModClauseContext context)
        {

            //return base.VisitModClause(context);
            string result = "";
            var childrens = context.children;
            int childCount = context.ChildCount;
            if (context.modifier()!=null)
            {
                //there are modifiers which we need to take care of like + means and ,- means negation,
                foreach (var child in childrens)
                {
                    var childType = child.GetType();
                    if (childType == typeof(LuceneParser.ModifierContext))
                    {
                        //var gg= child.GetType();
                        string childText = child.GetText();
                        switch (childText)
                        {
                            case "-":
                                result += " NOT ";
                                break;
                            case "!":
                                result += " NOT ";
                                break;
                            default:
                                result += "";
                                break;
                        }
                    }
                    else if (childType == typeof(LuceneParser.ClauseContext))
                    {
                        result += this.VisitClause(context.clause());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

            }
            else
            {
                result += this.VisitClause(context.clause());
            }


          
            return result;
        }

        public override string VisitModifier([NotNull] LuceneParser.ModifierContext context)
        {
            return base.VisitModifier(context);
        }
        public override string VisitClause([NotNull] LuceneParser.ClauseContext context)
        {
            //return base.VisitClause(context);
            string result = "";
            var childrens = context.children;
            int childCount = context.ChildCount;
            string fieldName = "";
            string term = "";
            foreach (var child in childrens)
            {
                var childType=child.GetType();
                if(childType == typeof(LuceneParser.FieldNameContext))
                {
                    fieldName = child.GetText();
                    //result += "log_json." + child.GetText() + "= ";
                }else if(childType == typeof(LuceneParser.TermContext))
                {
                    term = this.VisitTerm(context.term());
                    // result += term;
                    if (fieldName != "" && term.Contains("*"))
                    {
                        //form regex expression
                        result += "REGEX(log_json." + fieldName + ",\'" + term.Substring(1,term.Length-2) +"\')";
                    }
                    else
                    {
                        if (term.Contains("BETWEEN"))
                        {
                            result += "log_json." + fieldName  + term;
                        }
                        else
                        {
                            result += "log_json." + fieldName + "=" + term;
                        }
                       
                    }

                    fieldName = "";
                }
                else if (childType == typeof(LuceneParser.GroupingExprContext))
                {
                    result += this.VisitGroupingExpr((LuceneParser.GroupingExprContext)child);
                }
                else if (childType == typeof(LuceneParser.FieldRangeExprContext))
                {
                    result += " Range expression ";
                }
                else if (childType == typeof(Antlr4.Runtime.Tree.TerminalNodeImpl))
                {
                    continue;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return result;
  
        }

        public string handleWildcard(string input )
        {
            return "";
        }
        public override string VisitFieldRangeExpr([NotNull] LuceneParser.FieldRangeExprContext context)
        {
            return base.VisitFieldRangeExpr(context);
        }
        public override string VisitTerm([NotNull] LuceneParser.TermContext context)
        {
            //return base.VisitTerm(context);
            if (context.quotedTerm() != null)
            {
                string text = context.quotedTerm().GetText();
                return "\'"+text.Substring(1, text.Length - 2)+"\'";
            }else if (context.termRangeExpr() != null)
            {
                var termRange = context.termRangeExpr();

                string leftText = termRange.left.Text;
                string rightText= termRange.right.Text;

                float l_fNo,r_fNo;
                int l_iNo,r_iNo;
                if (int.TryParse(leftText, out l_iNo) && int.TryParse(rightText, out r_iNo))
                {
                    return " BETWEEN " + l_iNo + " AND " + r_iNo + " ";
                }
                else if (float.TryParse(leftText, out l_fNo) && float.TryParse(rightText, out r_fNo))
                {
                    return " BETWEEN " + l_fNo + " AND " + r_fNo + " ";
                }
                else
                {
                    return " BETWEEN \'" + leftText + "\' AND \'" + rightText + "\'";
                }

                
            }
            else
            {
                string text = context.GetText();
                float fNo;
                int iNo;
              
                if (int.TryParse(text, out iNo))
                {
                    return " " + iNo + " ";
                }
                else if (float.TryParse(text, out fNo))
                {
                    return " " + fNo + " ";
                }
                else
                {
                    return "\'" + text + "\'";
                }

               
                //throw new NotImplementedException();
            }
        }
        public override string VisitGroupingExpr([NotNull] LuceneParser.GroupingExprContext context)
        {
            //return base.VisitGroupingExpr(context);
            string result = "(";
            result += this.VisitQuery(context.query());
            return result += ")";
        }
        public override string VisitFieldName([NotNull] LuceneParser.FieldNameContext context)
        {
            return  base.VisitFieldName(context);
        }
        public override string VisitTermRangeExpr([NotNull] LuceneParser.TermRangeExprContext context)
        {
            return base.VisitTermRangeExpr(context);
        }
        public override string VisitQuotedTerm([NotNull] LuceneParser.QuotedTermContext context)
        {
            return base.VisitQuotedTerm(context);
        }
        public override string VisitFuzzy([NotNull] LuceneParser.FuzzyContext context)
        {
            //return base.VisitFuzzy(context);
            Console.WriteLine("Not Suported");
            return "Not Suported";
        }
    }
}

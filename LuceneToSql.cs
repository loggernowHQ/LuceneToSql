using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using practice.CustomAntlr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneToSql
{
    public  class LuceneToSql
    {
        public static string getWhereClause(string luceneQuery)
        {
            try
            {
                ICharStream inputStream = CharStreams.fromString(luceneQuery);
                LuceneLexer lexer = new LuceneLexer(inputStream);
                CommonTokenStream tokenStream = new CommonTokenStream(lexer);
                var customErrorListener = new QueryLanguageErrorListener();
                LuceneParser parser = new LuceneParser(tokenStream);
                parser.AddErrorListener(customErrorListener);
                parser.BuildParseTree = true;
                IParseTree tree = parser.topLevelQuery();
                CustomVisitor customVisitor = new CustomVisitor();
                string visitorResult = customVisitor.Visit(tree);
                return visitorResult;
            }
            catch (Exception e)
            {

                return "Lucene Query Not supported "+e.Message;
            }
           
        }
    }
}

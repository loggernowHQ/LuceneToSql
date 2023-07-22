using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practice.CustomAntlr
{
    public class QueryLanguageErrorListener :BaseErrorListener
    {
        public bool IsValid { get; private set; } = true;
        public int ErrorLocation { get; private set; } = -1;
        public string ErrorMessage { get; private set; }

        public override void ReportAmbiguity(
          Parser recognizer, DFA dfa,
          int startIndex, int stopIndex,
          bool exact, BitSet ambigAlts,
          ATNConfigSet configs)
        {
            IsValid = false;
        }

        public override void ReportAttemptingFullContext(
          Parser recognizer, DFA dfa,
          int startIndex, int stopIndex,
          BitSet conflictingAlts, SimulatorState conflictState)
        {
            IsValid = false;
        }

        public override void ReportContextSensitivity(
          Parser recognizer, DFA dfa,
          int startIndex, int stopIndex,
          int prediction, SimulatorState acceptState)
        {
            IsValid = false;
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            IsValid = false;
            ErrorLocation = ErrorLocation == -1 ? charPositionInLine : ErrorLocation;
            ErrorMessage = msg;
            //base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }

        //public override void SyntaxError(
        //  IRecognizer recognizer, IToken offendingSymbol,
        //  int line, int charPositionInLine,
        // string msg, RecognitionException e)
        //{
        //    IsValid = false;
        //    ErrorLocation = ErrorLocation == -1 ? charPositionInLine : ErrorLocation;
        //    ErrorMessage = msg;
        //}
    }
}

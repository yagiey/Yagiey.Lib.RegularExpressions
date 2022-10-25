using System;
using System.Collections.Generic;
using System.Linq;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis
{
    using DFA = IDeterministicFiniteAutomaton<char>;
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;

    public class RegularExpressionsGenerator
    {
		public static DFA SimpleConcatenation(string str, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(str))
            {
                const string ErrMsg = "string must not be null or empty.";
                throw new ArgumentException(ErrMsg, nameof(str));
            }

            DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>();
            for (int i = 0; i < str.Length; i++)
            {
                transitionMap.Add(
                    i,
                    new Dictionary<IInput, int> { { new Input(str[i]), i + 1 } }
                );
            }
            return new RegularExpression(0, new int[] { str.Length }, transitionMap, ignoreCase);
        }

        public static DFA Identifier()
        {
            //[a-zA-Z_]\w*

            IEnumerable<char> identifierHead =
                Enumerable.Empty<char>()
                .Concat(Enumerable.Range(0, 26).Select(it => Convert.ToChar('A' + it)))
                .Concat(Enumerable.Range(0, 26).Select(it => Convert.ToChar('a' + it)))
                .Append('_');

            IEnumerable<char> identifierChar =
                identifierHead
                .Concat(Enumerable.Range(0, 10).Select(it => Convert.ToChar('0' + it)));

            DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
            {
                {
                    0,
                    new Dictionary<IInput, int>(identifierHead.Select(it => KeyValuePair.Create((IInput)new Input(it), 1)))
                },
                {
                    1,
                    new Dictionary<IInput, int>(identifierChar.Select(it => KeyValuePair.Create((IInput)new Input(it), 1)))
                },
            };
            return new RegularExpression(0, new int[] { 1 }, transitionMap, false);
        }

        public static DFA TrueOrFalse(CharacterCaseRuleEnum rule)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>();
#pragma warning restore IDE0028 // Simplify collection initialization

            transitionMap.Add(0, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[0].Add(new Input('t'), 1);
                    transitionMap[0].Add(new Input('T'), 1);
                    transitionMap[0].Add(new Input('f'), 4);
                    transitionMap[0].Add(new Input('F'), 4);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[0].Add(new Input('T'), 1);
                    transitionMap[0].Add(new Input('F'), 4);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[0].Add(new Input('t'), 1);
                    transitionMap[0].Add(new Input('f'), 4);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[0].Add(new Input('T'), 1);
                    transitionMap[0].Add(new Input('F'), 4);
                    break;
                default:
                    transitionMap[0].Add(new Input('t'), 1);
                    transitionMap[0].Add(new Input('f'), 4);
                    break;
            }

            transitionMap.Add(1, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[1].Add(new Input('r'), 2);
                    transitionMap[1].Add(new Input('R'), 2);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[1].Add(new Input('R'), 2);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[1].Add(new Input('r'), 2);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[1].Add(new Input('r'), 2);
                    break;
                default:
                    transitionMap[1].Add(new Input('r'), 2);
                    break;
            }

            transitionMap.Add(2, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[2].Add(new Input('u'), 3);
                    transitionMap[2].Add(new Input('U'), 3);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[2].Add(new Input('U'), 3);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[2].Add(new Input('u'), 3);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[2].Add(new Input('u'), 3);
                    break;
                default:
                    transitionMap[2].Add(new Input('u'), 3);
                    break;
            }

            transitionMap.Add(3, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[3].Add(new Input('e'), 8);
                    transitionMap[3].Add(new Input('E'), 8);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[3].Add(new Input('E'), 8);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[3].Add(new Input('e'), 8);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[3].Add(new Input('e'), 8);
                    break;
                default:
                    transitionMap[3].Add(new Input('e'), 8);
                    break;
            }

            transitionMap.Add(4, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[4].Add(new Input('a'), 5);
                    transitionMap[4].Add(new Input('A'), 5);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[4].Add(new Input('A'), 5);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[4].Add(new Input('a'), 5);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[4].Add(new Input('a'), 5);
                    break;
                default:
                    transitionMap[4].Add(new Input('a'), 5);
                    break;
            }

            transitionMap.Add(5, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[5].Add(new Input('l'), 6);
                    transitionMap[5].Add(new Input('L'), 6);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[5].Add(new Input('L'), 6);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[5].Add(new Input('l'), 6);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[5].Add(new Input('l'), 6);
                    break;
                default:
                    transitionMap[5].Add(new Input('l'), 6);
                    break;
            }

            transitionMap.Add(6, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[6].Add(new Input('s'), 7);
                    transitionMap[6].Add(new Input('S'), 7);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[6].Add(new Input('S'), 7);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[6].Add(new Input('s'), 7);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[6].Add(new Input('s'), 7);
                    break;
                default:
                    transitionMap[6].Add(new Input('s'), 7);
                    break;
            }

            transitionMap.Add(7, new Dictionary<IInput, int>());
            switch (rule)
            {
                case CharacterCaseRuleEnum.Ignore:
                    transitionMap[7].Add(new Input('e'), 8);
                    transitionMap[7].Add(new Input('E'), 8);
                    break;
                case CharacterCaseRuleEnum.Upper:
                    transitionMap[7].Add(new Input('E'), 8);
                    break;
                case CharacterCaseRuleEnum.Lower:
                    transitionMap[7].Add(new Input('e'), 8);
                    break;
                case CharacterCaseRuleEnum.Pascal:
                    transitionMap[7].Add(new Input('e'), 8);
                    break;
                default:
                    transitionMap[7].Add(new Input('e'), 8);
                    break;
            }

            return new RegularExpression(0, new int[] { 8 }, transitionMap, false);
        }

        public static DFA ZeroOrOne()
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>();
#pragma warning restore IDE0028 // Simplify collection initialization
            transitionMap.Add(0, new Dictionary<IInput, int>());
            transitionMap[0].Add(new Input('1'), 1);
            transitionMap[0].Add(new Input('0'), 1);
            return new RegularExpression(0, new int[] { 1 }, transitionMap, false);
        }

        public static DFA IntegerNumber()
        {
            //[+-]?([1-9]\d*|0)

            DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
            {
                {
                    0,
                    new Dictionary<IInput, int> {
                        { new Input('+'), 1 },
                        { new Input('-'), 1 },
                        { new Input('0'), 2 },
                        { new Input('1'), 3 },
                        { new Input('2'), 3 },
                        { new Input('3'), 3 },
                        { new Input('4'), 3 },
                        { new Input('5'), 3 },
                        { new Input('6'), 3 },
                        { new Input('7'), 3 },
                        { new Input('8'), 3 },
                        { new Input('9'), 3 },
                    }
                },
                {
                    1,
                    new Dictionary<IInput, int> {
                        { new Input('0'), 2 },
                        { new Input('1'), 3 },
                        { new Input('2'), 3 },
                        { new Input('3'), 3 },
                        { new Input('4'), 3 },
                        { new Input('5'), 3 },
                        { new Input('6'), 3 },
                        { new Input('7'), 3 },
                        { new Input('8'), 3 },
                        { new Input('9'), 3 },
                    }
                },
                {
                    3,
                    new Dictionary<IInput, int> {
                        { new Input('0'), 3 },
                        { new Input('1'), 3 },
                        { new Input('2'), 3 },
                        { new Input('3'), 3 },
                        { new Input('4'), 3 },
                        { new Input('5'), 3 },
                        { new Input('6'), 3 },
                        { new Input('7'), 3 },
                        { new Input('8'), 3 },
                        { new Input('9'), 3 },
                    }
                },
            };
            return new RegularExpression(0, new int[] { 2, 3 }, transitionMap, false);
        }

        public static DFA RealNumber()
        {
            //(\-|\+)?(\d+(\.\d*)?|\.\d+)((e|E)(\-|\+)?\d+)?
            DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
            {
                {
                    0,
                    new Dictionary<IInput, int> {
                        { new Input('+'), 1 },
                        { new Input('-'), 1 },
                        { new Input('.'), 2 },
                        { new Input('0'), 7 },
                        { new Input('1'), 7 },
                        { new Input('2'), 7 },
                        { new Input('3'), 7 },
                        { new Input('4'), 7 },
                        { new Input('5'), 7 },
                        { new Input('6'), 7 },
                        { new Input('7'), 7 },
                        { new Input('8'), 7 },
                        { new Input('9'), 7 },
                    }
                },
                {
                    1,
                    new Dictionary<IInput, int> {
                        { new Input('.'), 2 },
                        { new Input('0'), 7 },
                        { new Input('1'), 7 },
                        { new Input('2'), 7 },
                        { new Input('3'), 7 },
                        { new Input('4'), 7 },
                        { new Input('5'), 7 },
                        { new Input('6'), 7 },
                        { new Input('7'), 7 },
                        { new Input('8'), 7 },
                        { new Input('9'), 7 },
                    }
                },
                {
                    2,
                    new Dictionary<IInput, int> {
                        { new Input('0'), 5 },
                        { new Input('1'), 5 },
                        { new Input('2'), 5 },
                        { new Input('3'), 5 },
                        { new Input('4'), 5 },
                        { new Input('5'), 5 },
                        { new Input('6'), 5 },
                        { new Input('7'), 5 },
                        { new Input('8'), 5 },
                        { new Input('9'), 5 },
                    }
                },
                {
                    3,
                    new Dictionary<IInput, int> {
                        { new Input('0'), 6 },
                        { new Input('1'), 6 },
                        { new Input('2'), 6 },
                        { new Input('3'), 6 },
                        { new Input('4'), 6 },
                        { new Input('5'), 6 },
                        { new Input('6'), 6 },
                        { new Input('7'), 6 },
                        { new Input('8'), 6 },
                        { new Input('9'), 6 },
                    }
                },
                {
                    4,
                    new Dictionary<IInput, int> {
                        { new Input('+'), 3 },
                        { new Input('-'), 3 },
                        { new Input('0'), 6 },
                        { new Input('1'), 6 },
                        { new Input('2'), 6 },
                        { new Input('3'), 6 },
                        { new Input('4'), 6 },
                        { new Input('5'), 6 },
                        { new Input('6'), 6 },
                        { new Input('7'), 6 },
                        { new Input('8'), 6 },
                        { new Input('9'), 6 },
                    }
                },
                {
                    5,
                    new Dictionary<IInput, int> {
                        { new Input('0'), 5 },
                        { new Input('1'), 5 },
                        { new Input('2'), 5 },
                        { new Input('3'), 5 },
                        { new Input('4'), 5 },
                        { new Input('5'), 5 },
                        { new Input('6'), 5 },
                        { new Input('7'), 5 },
                        { new Input('8'), 5 },
                        { new Input('9'), 5 },
                        { new Input('E'), 4 },
                        { new Input('e'), 4 },
                    }
                },
                {
                    6,
                    new Dictionary<IInput, int> {
                        { new Input('0'), 6 },
                        { new Input('1'), 6 },
                        { new Input('2'), 6 },
                        { new Input('3'), 6 },
                        { new Input('4'), 6 },
                        { new Input('5'), 6 },
                        { new Input('6'), 6 },
                        { new Input('7'), 6 },
                        { new Input('8'), 6 },
                        { new Input('9'), 6 },
                    }
                },
                {
                    7,
                    new Dictionary<IInput, int> {
                        { new Input('.'), 5 },
                        { new Input('0'), 7 },
                        { new Input('1'), 7 },
                        { new Input('2'), 7 },
                        { new Input('3'), 7 },
                        { new Input('4'), 7 },
                        { new Input('5'), 7 },
                        { new Input('6'), 7 },
                        { new Input('7'), 7 },
                        { new Input('8'), 7 },
                        { new Input('9'), 7 },
                        { new Input('E'), 4 },
                        { new Input('e'), 4 },
                    }
                },
            };
            return new RegularExpression(0, new int[] { 5, 6, 7 }, transitionMap, false);
        }
	}
}

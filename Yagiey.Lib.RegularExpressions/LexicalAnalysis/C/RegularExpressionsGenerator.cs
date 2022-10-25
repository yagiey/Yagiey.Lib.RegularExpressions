using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;
using Yagiey.Lib.RegularExpressions.Functions;

namespace Yagiey.Lib.RegularExpressions.LexicalAnalysis.C
{
	using DFA = IDeterministicFiniteAutomaton<char>;
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;

	public class RegularExpressionsGenerator
	{
		public static DFA LineComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0,
					new Dictionary<IInput, int> {
						{ new Input('/'), 1 },
					}
				},
				{
					1,
					new Dictionary<IInput, int> {
						{ new Input('/'), 2 },
					}
				},
				{
					2,
					new Dictionary<IInput, int> {
						{ new Input('\r'), 3 },
						{ new Input('\n'), 4 },
						{ new InputWithPredicate(
							new And<char>(
								new List<IUnaryFunctionObject<bool,char>>
								{
									new Not<char>(new Equal<char>('\r')),
									new Not<char>(new Equal<char>('\n')),
								}
							)
						), 2 },
					}
				},
				{
					3,
					new Dictionary<IInput, int> {
						{ new Input('\n'), 4 },
						{ new InputWithPredicate(new Not<char>(new Equal<char>('\n'))), 2 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 4 }, transitionMap, false);
		}

		public static DFA BlockComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<IInput, int>>
			{
				{
					0 ,
					new Dictionary<IInput, int> {
						{ new Input('/'), 1 },
					}
				},
				{
					1 ,
					new Dictionary<IInput, int> {
						{ new Input('*'), 2 },
					}
				},
				{
					2 ,
					new Dictionary<IInput, int> {
						{ new Input('*'), 3 },
						{ new InputWithPredicate(new Not<char>( new Equal<char>('*'))), 2 },
					}
				},
				{
					3 ,
					new Dictionary<IInput, int> {
						{ new Input('/'), 4 },
						{ new InputWithPredicate(new Not<char>( new Equal<char>('/'))), 2 },
					}
				},
			};
			return new RegularExpression(0, new int[] { 4 }, transitionMap, false);
		}
	}
}

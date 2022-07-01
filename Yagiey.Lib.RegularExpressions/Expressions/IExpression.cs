using System.Collections.Generic;
using Yagiey.Lib.RegularExpressions.Automata;

namespace Yagiey.Lib.RegularExpressions.Expressions
{
	using NFATransitionMap = IDictionary<int, IDictionary<IInput, IEnumerable<int>>>;

	internal interface IExpression
	{
		int Start
		{
			get;
		}

		int End
		{
			get;
		}

		string ToRegularExpression();

		IExpression CopyWithNewNode(IEnumerator<int> itorID);

		void AddTransition(NFATransitionMap transitionMap, IEnumerator<int> itorID);
	}
}

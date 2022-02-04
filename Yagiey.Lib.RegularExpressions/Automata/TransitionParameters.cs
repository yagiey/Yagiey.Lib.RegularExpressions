using System.Diagnostics.CodeAnalysis;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	internal struct TransitionParameters
	{
		public int Node
		{
			get;
			set;
		}

		public Input Input
		{
			get;
			set;
		}

		public TransitionParameters(int node, Input input)
		{
			Node = node;
			Input = input;
		}

		public override bool Equals([NotNullWhen(true)] object? obj)
		{
			if (obj == null || obj.GetType() != typeof(TransitionParameters))
			{
				return false;
			}

			TransitionParameters o = (TransitionParameters)obj;
			return this == o;
		}

		public override int GetHashCode()
		{
			return Node ^ Input.GetHashCode();
		}

		public static bool operator ==(TransitionParameters lhs, TransitionParameters rhs)
		{
			return lhs.Node == rhs.Node && lhs.Input == rhs.Input;
		}

		public static bool operator !=(TransitionParameters lhs, TransitionParameters rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", Node, Input);
		}
	}
}

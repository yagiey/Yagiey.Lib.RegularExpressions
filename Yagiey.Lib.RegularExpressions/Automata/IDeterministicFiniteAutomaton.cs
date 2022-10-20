using System.Collections.Generic;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	public interface IDeterministicFiniteAutomaton<T>
	{
		/// <summary>initialize this automaton</summary>
		void Reset();

		/// <summary>read a character then transition state</summary>
		/// <param name="input">input value</param>
		void MoveNext(T input);

		bool IsInitialState();

		/// <summary>check that current state is acceptable or not</summary>
		/// <returns>true: acceptable, false: not acceptable</returns>
		bool IsAcceptable();

		/// <summary>check that the state after read entire input is acceptable or not</summary>
		/// <returns>true: acceptable, false: not acceptable</returns>
		bool IsAcceptable(IEnumerable<T> source);

		/// <summary>check that current state is error or not</summary>
		/// <remarks>once an error occurs, there is no need to read the characters after that.</remarks>
		/// <returns>true: error, false: not error</returns>
		bool IsError();

		/// <summary>check the state is error or not when the state transitions with the given input character without state transition.</summary>
		/// <param name="input">input value</param>
		/// <returns>true: error, false: not error</returns>
		bool IsNextError(T input);

		/// <summary>check the state is acceptable or not when the state transitions with the given input character without state transition.</summary>
		/// <param name="input">input value</param>
		/// <returns>true: acceptable, false: not acceptable</returns>
		bool IsNextAcceptable(T input);
	}
}

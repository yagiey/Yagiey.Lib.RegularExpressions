using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yagiey.Lib.RegularExpressions.Extensions.Generic;

namespace Yagiey.Lib.RegularExpressions.Automata
{
	using DFATransitionMap = IDictionary<int, IDictionary<IInput, int>>;

	internal class DeterministicFiniteAutomaton : IDeterministicFiniteAutomaton<char>
	{
		private int _state;
		private bool _isError;

		public DeterministicFiniteAutomaton(int startNode, IEnumerable<int> acceptingNodeSet, DFATransitionMap transitionMap, bool ignoreCase)
		{
			StartNode = startNode;
			AcceptingNodeSet = acceptingNodeSet;
			TransitionMap = transitionMap;
			IgnoreCase = ignoreCase;
		}

		public bool IgnoreCase
		{
			get;
			private set;
		}

		public int StartNode
		{
			get;
			private set;
		}

		public IEnumerable<int> AcceptingNodeSet
		{
			get;
			private set;
		}

		public DFATransitionMap TransitionMap
		{
			get;
			private set;
		}

		public IEnumerable<int> GetAllNodes()
		{
			IEnumerable<int> result = Enumerable.Empty<int>();
			foreach (var pair1 in TransitionMap)
			{
				int node = pair1.Key;
				result = result.Append(node);
				IDictionary<IInput, int> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					int destination = pair2.Value;
					result = result.Append(destination);
				}
			}
			return result.Distinct().OrderBy(_ => _);
		}

		public static DFATransitionMap AddTransition(DFATransitionMap transitionMap, int node, IInput input, int dest)
		{
			if (transitionMap.ContainsKey(node))
			{
				var dic = transitionMap[node];
				dic.Add(input, dest);
			}
			else
			{
				IDictionary<IInput, int> dic = new Dictionary<IInput, int>
				{
					{ input, dest }
				};
				transitionMap.Add(node, dic);
			}
			return transitionMap;
		}

		public override string ToString()
		{
			List<string> lines = new()
			{
				string.Format("start: {0}", StartNode),
				string.Format("accepting: [{0}]", string.Join(",", AcceptingNodeSet))
			};

			foreach (var pair in TransitionMap.OrderBy(_ => _.Key))
			{
				var strMap = string.Join(",", pair.Value.OrderBy(_ => _.Key).Select(pair2 => string.Format("{0}->{1}", pair2.Key is Input ? string.Format("(ch {0})", (int)(pair2.Key as Input)!.Character) : pair2.Key.ToString(), pair2.Value)));
				lines.Add(string.Format("{0}:{1}", pair.Key, strMap));
			}
			return string.Join("\r\n", lines);
		}

		public string? ToRegularExpression()
		{
			var allNode = GetAllNodes();
			var gnfa = ToGNFA();

			int startNode = gnfa.Item1;
			int acceptingNode = gnfa.Item2;
			var gnfaTransMap = gnfa.Item3;

			List<int> done = new();
			foreach (var nodeRip in allNode)
			{
				done.Add(nodeRip);

				// each node
				var remain = allNode.Except(done);
				foreach (var pair in DirectProductEnumerator.DP(remain, remain))
				{
					UpdateEdgeLabel(pair.Item1, nodeRip, pair.Item2, gnfaTransMap);
				}

				// start -> node
				foreach (var to in remain)
				{
					UpdateEdgeLabel(startNode, nodeRip, to, gnfaTransMap);
				}

				// node -> accepting
				foreach (var from in remain)
				{
					UpdateEdgeLabel(from, nodeRip, acceptingNode, gnfaTransMap);
				}

				// start -> accepting
				UpdateEdgeLabel(startNode, nodeRip, acceptingNode, gnfaTransMap);

				// remove node
				gnfaTransMap.Remove(nodeRip);
				foreach (var item in gnfaTransMap)
				{
					item.Value.RemoveAll(it => it.Head == nodeRip);
				}
			}

			string? result = gnfaTransMap[startNode].First().Tail;
			return result;
		}

		private Tuple<int, int, IDictionary<int, List<Pair<int, string?>>>> ToGNFA()
		{
			var e =
				TransitionMap
					.Select(it1 =>
						KeyValuePair.Create(
							it1.Key,
							it1.Value.Select(it2 =>
								new Pair<int, string?>
								(
									it2.Value,
									it2.Key.ToRegularExpression()
								)
							).ToList()
						)
					);

			IDictionary<int, List<Pair<int, string?>>> newMap =
				new Dictionary<int, List<Pair<int, string?>>>(e);

			// step 1
			IEnumerable<int> allNode = GetAllNodes();
			int lastNode = allNode.Max();
			int newStart = lastNode + 1;
			int newAccepting = newStart + 1;

			// step 2: new start node -> each node
			newMap.Add(
				newStart,
				allNode.Select(it => new Pair<int, string?>(it, it == StartNode ? string.Empty : null)).ToList()
			);

			// step 3: each node -> new accepting state
			foreach (var node in allNode)
			{
				string? label = AcceptingNodeSet.Contains(node) ? string.Empty : null;
				if (newMap.TryGetValue(node, out List<Pair<int, string?>>? tmp) && tmp != null)
				{
					tmp.Add(new Pair<int, string?>(newAccepting, label));
				}
				else
				{
					newMap.Add(
						node,
						new List<Pair<int, string?>> { new Pair<int, string?>(newAccepting, label) }
					);
				}
			}
			newMap[newStart].Add(new Pair<int, string?>(newAccepting, null));

			// step 4: each node -> each node
			foreach (var node1 in allNode)
			{
				foreach (var node2 in allNode)
				{
					// node1 -> node2
					if (newMap.TryGetValue(node1, out List<Pair<int, string?>>? lis)
						&& lis != null)
					{
						var tmp = lis.Where(it => it.Head == node2);
						if (tmp.Any())
						{
							var tmp2 = tmp.ToList();
							lis.RemoveAll(it => it.Head == node2);
							string label = string.Join("|", tmp2.Select(it => it.Tail));
							lis.Add(new Pair<int, string?>(node2, label));
						}
						else
						{
							lis.Add(new Pair<int, string?>(node2, null));
						}
					}
					else
					{
						newMap.Add(
							node1,
							Enumerable.Repeat(new Pair<int, string?>(node2, null), 1).ToList()
						);
					}
				}
			}
			return Tuple.Create(newStart, newAccepting, newMap);
		}

		private static bool IsNoPath(string? label)
		{
			return label is null;
		}

		private static bool IsEmpty(string? label)
		{
			return (label is not null) && label.Length == 0;
		}

		private static void UpdateEdgeLabel(int from, int rip, int to, IDictionary<int, List<Pair<int, string?>>> transitionMap)
		{
			var r1 = transitionMap[from].First(it => it.Head == rip);
			var r2 = transitionMap[rip].First(it => it.Head == rip);
			var r3 = transitionMap[rip].First(it => it.Head == to);
			var r4 = transitionMap[from].First(it => it.Head == to);

			string? result;
			if (IsNoPath(r1.Tail) || IsNoPath(r3.Tail))
			{
				result = r4.Tail;
			}
			else
			{
				string strR1;
				if (IsEmpty(r1.Tail))
				{
					strR1 = string.Empty;
				}
				else
				{
					string tmp1 = SimplifyExpression(r1.Tail)!;
					strR1 = tmp1.Contains('|') ? $"({r1.Tail!})" : r1.Tail!;
				}

				string strR2;
				if (IsNoPath(r2.Tail))
				{
					strR2 = string.Empty;
				}
				else
				{
					string tmp2 = SimplifyExpression(r2.Tail)!;
					strR2 = 1 < tmp2.Length ? $"({r2.Tail!})*" : $"{r2.Tail!}*";
				}

				string strR3;
				if (IsEmpty(r3.Tail))
				{
					strR3 = string.Empty;
				}
				else
				{
					string tmp3 = SimplifyExpression(r3.Tail)!;
					strR3 = tmp3.Contains('|') ? $"({r3.Tail!})" : r3.Tail!;
				}

				if (IsEmpty(r4.Tail))
				{
					result = $"({strR1}{strR2}{strR3})?";
				}
				else
				{
					string strR4;
					if (IsNoPath(r4.Tail))
					{
						strR4 = string.Empty;
					}
					else
					{
						string tmp4 = SimplifyExpression(r4.Tail)!;
						strR4 = tmp4.Contains('|') ? $"|({r4.Tail})" : $"|{r4.Tail}";
					}

					result = $"{strR1}{strR2}{strR3}{strR4}";
				}
			}

			r4.Tail = result;
		}

		private static IEnumerable<Pair<int, int>> ParenIndexices(string expr, IEnumerable<Tuple<char, char, bool>> parenInfos)
		{
			IList<Tuple<char, bool>> opening =
				parenInfos
				.Select(it => Tuple.Create(it.Item1, it.Item3))
				.ToList();

			IList<Tuple<char, bool>> closing =
				parenInfos
				.Select(it => Tuple.Create(it.Item2, it.Item3))
				.ToList();

			Stack<Tuple<int, int>> stk = new();
			bool escape = false;
			for (int i = 0; i < expr.Length; i++)
			{
				char ch = expr[i];
				if (ch == Constants.Backslash)
				{
					escape = true;
					continue;
				}

				if (!escape)
				{
					bool canSkip = stk.Any() && opening[stk.Peek().Item2].Item2;
					if (opening.Any(it => it.Item1 == ch))
					{
						////////////
						// opening
						////////////

						int parenType = opening.FindIndex(it => it.Item1 == ch);

						if (!canSkip)
						{
							Tuple<int, int> posParen = Tuple.Create(i, parenType);
							stk.Push(posParen);
						}
					}
					else if (closing.Any(it => it.Item1 == ch))
					{
						////////////
						// closing
						////////////

						if (!stk.Any())
						{
							const string ErrMsg = "too many closing parentheses";
							throw new Exception(ErrMsg);
						}

						int parenType = closing.FindIndex(it => it.Item1 == ch);

						Tuple<int, int> peek = stk.Peek();
						if (!canSkip || canSkip && parenType == peek.Item2)
						{
							if (peek.Item2 != parenType)
							{
								const string ErrMsg = "closing with different parentheses";
								throw new Exception(ErrMsg);
							}
							stk.Pop();
							yield return new Pair<int, int>(peek.Item1, i);
						}
					}
				}

				escape = false;
			}

			if (stk.Count > 0)
			{
				const string ErrMsg = "unclosed parentheses";
				throw new Exception(ErrMsg);
			}
		}

		/// <summary>
		/// Replace outermost expressions which enclosed in parentheses or brackets with a character
		/// </summary>
		/// <param name="expr">regular expression</param>
		/// <returns>replaced string</returns>
		private static string? SimplifyExpression(string? expr)
		{
			if (expr is null)
			{
				return null;
			}

			const char Replace = 'A';
			var parens = new[] { Tuple.Create('(', ')', false), Tuple.Create('[', ']', true) };
			var pos = ParenIndexices(expr, parens).OrderBy(it => it.Head);
			StringBuilder sb = new();

			int prevEnd = -1;
			bool isFirst = true;
			foreach (var startAndEnd in pos)
			{
				int start = startAndEnd.Head;
				int end = startAndEnd.Tail;

				if (prevEnd <= end)
				{
					string sub;
					if (isFirst)
					{
						sub = expr.Substring(prevEnd + 1, start);
					}
					else
					{
						sub = expr.Substring(prevEnd + 1, start - prevEnd - 1);
					}
					sb.Append(sub);
					isFirst = false;

					int len = end - start + 1;
					sb.Append(Replace);
					prevEnd = end;
				}
			}
			sb.Append(expr.Substring(prevEnd + 1));
			return sb.ToString();
		}

		#region IDeterministicFiniteAutomaton

		public void Reset()
		{
			Reset(0, false);
		}

		private void Reset(int state, bool isError)
		{
			_state = state;
			_isError = isError;
		}

		public void MoveNext(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			if (result.Item1)
			{
				_state = result.Item2;
				return;
			}

			if (IgnoreCase)
			{
				char? corr = ToCorrespondingCase(ch);
				if (corr.HasValue)
				{
					var result2 = GetNext(corr.Value, _state, TransitionMap, IsError());
					if (result2.Item1)
					{
						_state = result2.Item2;
						return;
					}
				}
			}

			_isError = true;
		}

		private static Tuple<bool, int> GetNext(char ch, int current, DFATransitionMap transitionMap, bool isError)
		{
			if (isError)
			{
				return Tuple.Create(false, 0);
			}

			bool result = transitionMap.TryGetValue(current, out IDictionary<IInput, int>? dic);
			if (!result || dic == null)
			{
				return Tuple.Create(false, 0);
			}
			else
			{
				result = dic.TryGetValue(new Input(ch), out int dest);
				if (result)
				{
					return Tuple.Create(true, dest);
				}
				else
				{
					var dic2 = GetTransitionsWithPredicate(dic);
					foreach (var pair in dic2)
					{
						IInput key = pair.Key;
						if (key.Match(ch))
						{
							return Tuple.Create(true, pair.Value);
						}
					}
					return Tuple.Create(false, 0);
				}
			}
		}

		private static IDictionary<IInput, int> GetTransitionsWithPredicate(IDictionary<IInput, int> map)
		{
			return new Dictionary<IInput, int>(map.Where(_ => _.Key.IsPredicate));
		}

		public bool IsInitialState()
		{
			return _state == 0;
		}

		public bool IsAcceptable()
		{
			return !IsError() && AcceptingNodeSet.Contains(_state);
		}

		public bool IsAcceptable(IEnumerable<char> source)
		{
			int oldState = _state;
			bool oldIsError = _isError;
			foreach (char ch in source)
			{
				MoveNext(ch);
				if (_isError)
				{
					Reset(oldState, oldIsError);
					return false;
				}
			}
			bool result = IsAcceptable();
			Reset(oldState, oldIsError);
			return result;
		}

		public bool IsError()
		{
			return _isError;
		}

		public bool IsNextError(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			return !result.Item1;
		}

		public bool IsNextAcceptable(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			return result.Item1 && AcceptingNodeSet.Contains(result.Item2);
		}

		#endregion

		private static char? ToCorrespondingCase(char ch)
		{
			if (IsUpperCase(ch))
			{
				return char.ToLower(ch);
			}
			else if (IsLowerCase(ch))
			{
				return char.ToUpper(ch);
			}
			else
			{
				return null;
			}
		}

		private static bool IsUpperCase(char ch)
		{
			return 'A' <= ch && ch <= 'Z';
		}

		private static bool IsLowerCase(char ch)
		{
			return 'a' <= ch && ch <= 'z';
		}
	}
}

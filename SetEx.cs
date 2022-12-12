using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace XLibrary
{
	public class SetEx<T> : ICollection<T>
	{
		public class Node
		{
			public override string ToString()
			{
				return $"{value};{count},{changeCount},{isRed};{left?.value.ToString() ?? "null"},{right?.value.ToString() ?? "null"};{previous?.value.ToString() ?? "null"},{next?.value.ToString() ?? "null"}";
			}
			private T value;
			public T Item { get => value; set => this.value = value; }
			private Node left;
			private Node right;
			internal Node previous;
			internal Node next;
			public Node Left { get => left; }
			public Node Right { get => right; }
			public Node Previous { get => previous; }
			public Node Next { get => next; }
			internal bool isRed;
			public bool IsBlack
			{
				get => !isRed;
			}
			public bool IsRed
			{
				get => isRed;
			}
			internal void ColorBlack() => isRed = false;
			internal void ColorRed() => isRed = true;
			internal void SetColor(bool isRed) => this.isRed = isRed;
			private int count = 1;
			private int changeCount = 0;
			internal bool countUpdated = false;
			private int UpdateChange()
			{
				CheckCount();
				int change = changeCount;
				changeCount = 0;
				count += change;
				return change;
			}
			private int CheckCount()
			{
				if (!countUpdated)
				{
					left?.CheckCount();
					right?.CheckCount();
					changeCount += left?.UpdateChange() ?? 0;
					changeCount += right?.UpdateChange() ?? 0;
					countUpdated = true;
				}
				return count + changeCount;
			}
			public int Count { get => CheckCount(); }
			public Node GetChild(bool atLeft)
			{
				if (atLeft) return Left;
				return Right;
			}
			public Node GetSibling(Node child)
			{
				if (child.IsAtLeft(this)) return Right;
				else return Left;
			}
			public bool IsAtLeft(Node Parent) => ReferenceEquals(Parent.Left, this);
			internal Node SetLeft(Node New)
			{
				changeCount += Left?.UpdateChange() ?? 0;
				changeCount -= Left?.Count ?? 0;
				Node node = Left;
				left = New;
				changeCount += Left?.UpdateChange() ?? 0;
				changeCount += Left?.Count ?? 0;
				countUpdated = false;
				return node;
			}
			internal Node SetRight(Node New)
			{
				changeCount += Right?.UpdateChange() ?? 0;
				changeCount -= Right?.Count ?? 0;
				Node node = Right;
				right = New;
				changeCount += Right?.UpdateChange() ?? 0;
				changeCount += Right?.Count ?? 0;
				countUpdated = false;
				return node;
			}
			internal Node ReplaceChild(bool atleft, Node New)
			{
				if (atleft) return SetLeft(New);
				else return SetRight(New);
			}
			internal Node ReplaceChild(Node child, Node New)
			{
				if (ReferenceEquals(Left, child)) return SetLeft(New);
				else return SetRight(New);
			}
			internal Node RotateLeft(Func<Node, Node, Node> ReplaceFn)
			{
				//Node node = right;
				//right = node.left;
				//node.left = this;
				//return node;
				Node node = SetRight(null);
				SetRight(node.SetLeft(ReplaceFn(this, node)));
				return node;
			}
			internal Node RotateLeftRight(Func<Node, Node, Node> ReplaceFn)
			{
				//Node child = SetLeft(null);
				//Node grandChild = child.SetRight(null);
				////left = grandChild.right;
				//SetLeft(grandChild.Right);
				////grandChild.right = this;
				//grandChild.SetRight(this);
				////child.right = grandChild.left;
				//child.SetRight(grandChild.Left);
				////grandChild.left = child;
				//grandChild.SetLeft(child);
				//child.SetRight(grandChild.SetLeft(SetLeft(grandChild.SetRight(ReplaceFn(this, grandChild)))));
				//return grandChild;
				Left.RotateLeft(ReplaceChild);
				return RotateRight(ReplaceFn);

			}
			internal Node RotateRight(Func<Node, Node, Node> ReplaceFn)
			{
				//Node node = left;
				//left = node.right;
				//node.right = this;
				//return node;
				Node node = SetLeft(null);
				SetLeft(node.SetRight(ReplaceFn(this, node)));
				return node;
			}
			internal Node RotateRightLeft(Func<Node, Node, Node> ReplaceFn)
			{
				//Node child = SetRight(null);
				//Node grandChild = child.SetLeft(null);
				////right = grandChild.left;
				//SetRight(grandChild.Left);
				////grandChild.left = this;
				//grandChild.SetLeft(this);
				////child.left = grandChild.right;
				//child.SetLeft(grandChild.Right);
				////grandChild.right = child;
				//child.SetLeft(grandChild.SetRight(SetRight(grandChild.SetLeft(ReplaceFn(this, grandChild)))));
				//return grandChild;
				Right.RotateRight(ReplaceChild);
				return RotateLeft(ReplaceFn);
			}
			public static bool IsNullOrBlack(Node node) => node == null || node.IsBlack;
			public static bool IsNonNullRed(Node node) => node != null && node.IsRed;
			public static bool IsNonNullBlack(Node node) => node != null && node.IsBlack;
			internal bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);
			internal bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);
			internal void Split4Node()
			{
				ColorRed();
				Left.ColorBlack();
				Right.ColorBlack();
			}
			internal void Merge2Nodes()
			{
				ColorBlack();
				Left.ColorRed();
				Right.ColorRed();
			}
			internal Node Rotate(Node current, Node sibling, Func<Node, Node, Node> func)
			{
				bool currentIsLeftChild = Left == current;
				Node removeRed;
				if (IsNonNullRed(sibling.Left))
				{
					if (currentIsLeftChild)
					{
						return RotateRightLeft(func);
					}
					else
					{
						removeRed = Left.Left;
						removeRed.ColorBlack();
						return RotateRight(func);
					}
				}
				else
				{
					if (currentIsLeftChild)
					{
						removeRed = Right.Right;
						removeRed.ColorBlack();
						return RotateLeft(func);
					}
					else
					{
						return RotateLeftRight(func);
					}

				}
			}
			public int ToTreeString(List<string> L, int l = 0, int p = 0)
			{
				int p2 = p;
				if (L.Count <= l) L.Add("");
				string valueStr = "|" + (isRed ? "r" : "b") + (value?.ToString() ?? "null");
				if (L[l].Length <= p) L[l] += Utils.Repeat(" ", p - L[l].Length + 1);
				L[l] = L[l].Insert(p, valueStr);
				p += valueStr.Length;
				if (Left != null)
				{
					p2 = Left.ToTreeString(L, l + 1, p2);
				}
				else
				{
					if (L.Count <= l) L.Add("|");
					p2 += 1;
				}
				if (Right != null)
				{
					p2 = Right.ToTreeString(L, l + 1, p2);
				}
				else
				{
					if (L.Count <= l) L.Add("|");
					p2 += 1;
				}
				return Utils.Max(p, p2);
			}
			public Node(T value, bool isRed)
			{
				this.value = value;
				this.isRed = isRed;
			}
		}
		private Node root;
		private Node first;
		private Node last;
		private void Link(Node a, Node b)
		{
			if (a != null) a.next = b;
			else first = b;
			if (b != null) b.previous = a;
			else last = a;
		}
		private void Link(Node a, Node b, Node c)
		{
			Link(a, b); Link(b, c);
		}
		private void RemoveLink(Node a)
		{
			Link(a.previous, a.next);
		}
		private Node NewToTree(Node New, Node current, bool atleft)
		{
			if (atleft)
			{
				current.SetLeft(New);
				Link(current.previous, New, current);
			}
			else
			{
				current.SetRight(New);
				Link(current, New, current.next);
			}
			New.SetColor(true);
			return New;
		}
		public Node Root { get => root; }
		private readonly IComparer<T> comparer;
		public SetEx(ICollection<T> c = null, IComparer<T> comparer = null)
		{
			this.comparer = comparer ?? Comparer<T>.Default;
			if (c != null)
				foreach (var i in c) DoAdd(i);
		}
		public int Count => root?.Count ?? 0;
		public bool IsReadOnly => false;
		public Node Last => last;
		public Node First => first;
		public virtual Node Find(T item)
		{
			Node node = root;
			while (node != null)
			{
				int order = comparer.Compare(item, node.Item);
				if (order == 0) return node;
				else if (order > 0) node = node.Right;
				else node = node.Left;
			}
			return node;
		}
		public (Node, int) FindClosest(T item)
		{
			Node node = root;
			int order = 0;
			while (node != null)
			{
				order = comparer.Compare(item, node.Item);
				if (order == 0) break;
				if (order < 0) if (node.Left == null) break; else node = node.Left;
				else if (node.Right == null) break; else node = node.Right;
			}
			return (node, order);
		}
		public virtual Node DoAdd(T item)
		{
			if (root == null)
			{
				// The tree is empty and this is the first item.
				root = new Node(item, false);
				return root;
			}

			// Search for a node at bottom to insert the new node.
			// If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
			// We split 4-nodes along the search path.
			Node current = root;
			Node parent = null;
			Node grandParent = null;
			Node greatGrandParent = null;

			// Even if we don't actually add to the set, we may be altering its structure (by doing rotations and such).
			// So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.

			int order = 0;
			while (current != null)
			{
				order = comparer.Compare(item, current.Item);
				if (order == 0)
				{
					// We could have changed root node to red during the search process.
					// We need to set it to black before we return.
					root.ColorBlack();
					return null;
				}

				// Split a 4-node into two 2-nodes.
				if (current.Is4Node)
				{
					current.Split4Node();
					// We could have introduced two consecutive red nodes after split. Fix that by rotation.
					if (Node.IsNonNullRed(parent))
					{
						InsertionBalance(current, ref parent, grandParent!, greatGrandParent!);
					}
				}
				current.countUpdated = false;

				greatGrandParent = grandParent;
				grandParent = parent;
				parent = current;
				current = (order < 0) ? current.Left : current.Right;
			}

			Debug.Assert(parent != null);
			// We're ready to insert the new node.
			Node node = NewToTree(new Node(item, true), parent, order < 0);
			//if (order > 0)
			//{
			//	//parent.Right = node;
			//	parent.SetRight(node);
			//}
			//else
			//{
			//	//parent.Left = node;
			//	parent.SetLeft(node);
			//}
			// The new node will be red, so we will need to adjust colors if its parent is also red.
			if (parent.IsRed)
			{
				InsertionBalance(node, ref parent!, grandParent!, greatGrandParent!);
			}

			// The root node is always black.
			root.ColorBlack();
			return node;
		}
		public virtual Node DoRemove(T item)
		{
			if (root == null)
			{
				return null;
			}

			// Search for a node and then find its successor.
			// Then copy the item from the successor to the matching node, and delete the successor.
			// If a node doesn't have a successor, we can replace it with its left child (if not empty),
			// or delete the matching node.
			//
			// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
			// Following code will make sure the node on the path is not a 2-node.

			// Even if we don't actually remove from the set, we may be altering its structure (by doing rotations
			// and such). So update our version to disable any enumerators/subsets working on it.

			Node current = root;
			Node parent = null;
			Node grandParent = null;
			Node match = null;
			Node parentOfMatch = null;
			bool foundMatch = false;
			while (current != null)
			{
				if (current.Is2Node)
				{
					// Fix up 2-node
					if (parent == null)
					{
						// `current` is the root. Mark it red.
						current.ColorRed();
					}
					else
					{
						Node sibling = parent.GetSibling(current);
						if (sibling.IsRed)
						{
							// If parent is a 3-node, flip the orientation of the red link.
							// We can achieve this by a single rotation.
							// This case is converted to one of the other cases below.
							Debug.Assert(parent.IsBlack);
							var ReplaceFn = ReplaceChildOrRootFn(grandParent);
							if (parent.Right == sibling)
							{
								parent.RotateLeft(ReplaceFn);
							}
							else
							{
								parent.RotateRight(ReplaceFn);
							}

							parent.ColorRed();
							sibling.ColorBlack(); // The red parent can't have black children.
												  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
												  //ReplaceChildOrRoot(grandParent, parent, sibling);
												  // `sibling` will become the grandparent of `current`.
							grandParent = sibling;
							if (parent == match)
							{
								parentOfMatch = sibling;
							}

							sibling = parent.GetSibling(current);
						}

						Debug.Assert(Node.IsNonNullBlack(sibling));

						if (sibling.Is2Node)
						{
							parent.Merge2Nodes();
						}
						else
						{
							// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
							// We can change the color of `current` to red by some rotation.
							Node newGrandParent = parent.Rotate(current, sibling, ReplaceChildOrRootFn(grandParent))!;

							newGrandParent.SetColor(parent.IsRed);
							parent.ColorBlack();
							current.ColorRed();

							//ReplaceChildOrRoot(grandParent, parent, newGrandParent);
							if (parent == match)
							{
								parentOfMatch = newGrandParent;
							}
							grandParent = newGrandParent;
						}
					}
				}

				// We don't need to compare after we find the match.
				int order = foundMatch ? -1 : comparer.Compare(item, current.Item);
				if (order == 0)
				{
					// Save the matching node.
					foundMatch = true;
					match = current;
					parentOfMatch = parent;
				}

				grandParent = parent;
				parent = current;
				// If we found a match, continue the search in the right sub-tree.
				current.countUpdated = false;
				current = order < 0 ? current.Left : current.Right;
			}

			// Move successor to the matching node position and replace links.
			if (match != null)
			{
				ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
			}

			root?.ColorBlack();
			RemoveLink(match);
			return match;
		}
		// After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
		// It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
		// need to split again in the next node.
		// By the time we need to split again, everything will be correctly set.
		private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
		{
			Debug.Assert(parent != null);
			Debug.Assert(grandParent != null);

			bool parentIsOnRight = grandParent.Right == parent;
			bool currentIsOnRight = parent.Right == current;

			Node newChildOfGreatGrandParent;
			var ReplaceFn = ReplaceChildOrRootFn(greatGrandParent);
			if (parentIsOnRight == currentIsOnRight)
			{
				// Same orientation, single rotation
				newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft(ReplaceFn) : grandParent.RotateRight(ReplaceFn);
			}
			else
			{
				// Different orientation, double rotation
				newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight(ReplaceFn) : grandParent.RotateRightLeft(ReplaceFn);
				// Current node now becomes the child of `greatGrandParent`
				parent = greatGrandParent;
			}

			// `grandParent` will become a child of either `parent` of `current`.
			grandParent.ColorRed();
			newChildOfGreatGrandParent.ColorBlack();

			//ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
		}
		/// <summary>
		/// Replaces the child of a parent node, or replaces the root if the parent is <c>null</c>.
		/// </summary>
		/// <param name="parent">The (possibly <c>null</c>) parent.</param>
		/// <param name="child">The child node to replace.</param>
		/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
		private Node ReplaceChildOrRoot(Node parent, Node child, Node newChild)
		{
			if (parent != null)
			{
				return parent.ReplaceChild(child, newChild);
			}
			else
			{
				root = newChild;
				return child;
			}
		}
		private Func<Node, Node, Node> ReplaceChildOrRootFn(Node parent) => (c, n) => ReplaceChildOrRoot(parent, c, n);
		/// <summary>
		/// Replaces the matching node with its successor.
		/// </summary>
		private void ReplaceNode(Node match, Node parentOfMatch, Node successor, Node parentOfSuccessor)
		{
			Debug.Assert(match != null);

			if (successor == match)
			{
				// This node has no successor. This can only happen if the right child of the match is null.
				Debug.Assert(match.Right == null);
				successor = match.Left!;
			}
			else
			{
				Debug.Assert(parentOfSuccessor != null);
				Debug.Assert(successor.Left == null);
				Debug.Assert((successor.Right == null && successor.IsRed) || (successor.Right!.IsRed && successor.IsBlack));

				successor.Right?.ColorBlack();

				if (parentOfSuccessor != match)
				{
					// Detach the successor from its parent and set its right child.

					//parentOfSuccessor.Left = successor.Right;
					//successor.Right = match.Right;
					parentOfSuccessor.SetLeft(successor.SetRight(match.SetRight(null)));
				}

				//successor.Left = match.Left;
				successor.SetLeft(match.SetLeft(null));
			}

			if (successor != null)
			{
				//successor.Color = match.Color;
				successor.SetColor(match.IsRed);
			}

			ReplaceChildOrRoot(parentOfMatch, match, successor!);
		}
		public void Add(T item)
		{
			DoAdd(item);
		}
		public void Clear()
		{
			root = first = last = null;
		}
		public bool Contains(T item) => Find(item) != null;
		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		public bool Remove(T item) => DoRemove(item) != null;
		public IEnumerator<T> GetEnumerator()
		{
			Node node = First;
			while (node != null)
			{
				yield return node.Item;
				node = node.next;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public virtual Node FindByIndex(int index)
		{
			Node node = root;
			while (node != null)
			{
				if (node.Count == index) return node;
				else if (node.Count > index) node = node.Right;
				index -= node.Count + 1;
				node = node.Left;
			}
			return node;
		}
		public virtual int GetIndex(T item)
		{
			Node node = root;
			int index = 0;
			while (node != null)
			{
				int order = comparer.Compare(item, node.Item);
				if (order == 0) return index;
				else if (order < 0) node = node.Left;
				index += node.Count + 1;
				node = node.Right;
			}
			return index;
		}
		public Node FindNotGreater(T item)
		{
			var V = FindClosest(item);
			if (V.Item2 > 0) return V.Item1.Previous;
			return V.Item1;
		}
		public Node FindGreater(T item)
		{
			var V = FindClosest(item);
			if (V.Item2 <= 0) return V.Item1.Next;
			return V.Item1;
		}
		public override string ToString()
		{
			List<string> res = new List<string>();
			root.ToTreeString(res);
			return string.Join("\n", res);
		}
		public virtual void ForEach(Action<Node> action)
		{
			Node node = First;
			while (node != null)
			{
				action(node);
				node = node.next;
			}
		}
	}

	public class MapEx<TKey, TValue> : SetEx<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
	{
		public TValue this[TKey key]
		{
			get
			{
				Node node = Find(key);
				if (node != null) return node.Item.Value;
				else return default;
			}
			set
			{

				Node node = Find(key);
				if (node != null) node.Item = new KeyValuePair<TKey, TValue>(key, value);
				else DoAdd(key, value);
			}
		}
		public ICollection<TKey> Keys
		{
			get
			{
				List<TKey> L = new();
				foreach (var i in this)
				{
					L.Add(i.Key);
				}
				return L;
			}
		}
		public ICollection<TValue> Values
		{
			get
			{
				List<TValue> L = new();
				foreach (var i in this)
				{
					L.Add(i.Value);
				}
				return L;
			}
		}
		public void Add(TKey key, TValue value)
		{
			DoAdd(key, value);
		}
		public bool ContainsKey(TKey key)
		{
			return Find(key) != null;
		}
		public bool Remove(TKey key)
		{
			return DoRemove(key) != null;
		}
		public Node DoRemove(TKey key) => DoRemove(new KeyValuePair<TKey, TValue>(key, default));
		public Node DoAdd(TKey key, TValue value) => DoAdd(new KeyValuePair<TKey, TValue>(key, default));
		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
		{
			Node node = Find(key);
			if (node == null)
			{
				value = default;
				return false;
			}
			value = node.Item.Value;
			return true;
		}
		public Node Find(TKey key) => Find(new KeyValuePair<TKey, TValue>(key, default));
	}



}

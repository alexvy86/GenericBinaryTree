using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GenericBinaryTree
{
	public class BTree<TKey> where TKey : IComparable
	{
		class BTreeNode<TKey> where TKey : IComparable
		{
			public BTreeNode<TKey> Left;
			public BTreeNode<TKey> Right;
			public BTreeNode<TKey> Parent;
			public TKey Data;

			public BTreeNode()
			{
				Left = null;
				Right = null;
				Parent = null;
			}
		}

		BTreeNode<TKey> Root;

		public BTree()
		{
			Root = null;
		}

		public void Add(TKey Value)
		{
			BTreeNode<TKey> child = new BTreeNode<TKey>();
			child.Data = Value;

			// Is the tree empty? Make the root the new child
			if (Root == null)
			{
				Root = child;
			}
			else
			{
				// Start from the root of the tree
				BTreeNode<TKey> Iterator = Root;
				while (true)
				{
					// Compare the value to insert with the value in the current tree node
					int Compare = Value.CompareTo(Iterator.Data);
					// The value is smaller or equal to the current node, we need to store it on the left side
					// We test for equivalence as we allow duplicates (!)
					if (Compare <= 0)
						if (Iterator.Left != null)
						{
							// Travel further left
							Iterator = Iterator.Left;
							continue;
						}
						else
						{
							// An empty left leg, add the new node on the left leg
							Iterator.Left = child;
							child.Parent = Iterator;
							break;
						}
					if (Compare > 0)
						if (Iterator.Right != null)
						{
							// Continue to travel right
							Iterator = Iterator.Right;
							continue;
						}
						else
						{
							// Add the child to the right leg
							Iterator.Right = child;
							child.Parent = Iterator;
							break;
						}
				}

			}

		}

		/// <summary>
		/// This routine walks through the tree to see if the value given can be found.
		/// </summary>
		/// <param name="Value">The value to look for in the tree</param>
		/// <returns>True if found, False if not found</returns>

		public bool Find(TKey Value)
		{
			BTreeNode<TKey> Iterator = Root;
			while (Iterator != null)
			{
				int Compare = Value.CompareTo(Iterator.Data);
				// Did we find the value ?
				if (Compare == 0) return true;
				if (Compare < 0)
				{
					// Travel left
					Iterator = Iterator.Left;
					continue;
				}
				// Travel right
				Iterator = Iterator.Right;
			}
			return false;
		}

		/// <summary>
		/// Given a starting node, this routine will locate the left most node in the sub-tree
		/// If no further nodes are found, it returns the starting node
		/// </summary>
		/// <param name="start">The sub-tree starting point</param>
		/// <returns></returns>

		BTreeNode<TKey> FindMostLeft(BTreeNode<TKey> start)
		{
			BTreeNode<TKey> node = start;
			while (true)
			{
				if (node.Left != null)
				{
					node = node.Left;
					continue;
				}
				break;
			}
			return node;
		}

		/// <summary>
		/// Returns a list iterator of the elements in the tree implementing the IENumerator interface.
		/// </summary>
		/// <returns>IENumerator</returns>

		public IEnumerator<TKey> GetEnumerator()
		{
			return new BinaryTreeEnumerator(this);
		}

		/// <summary>
		/// The BinaryTreeEnumerator implements the IEnumerator allowing foreach enumeration of the tree
		/// </summary>

		class BinaryTreeEnumerator : IEnumerator<TKey>
		{
			BTreeNode<TKey> current;
			BTree<TKey> theTree;

			public BinaryTreeEnumerator(BTree<TKey> tree)
			{
				theTree = tree;
				current = null;
			}

			/// <summary>
			/// The MoveNext function traverses the tree in sorted order.
			/// </summary>
			/// <returns>True if we found a valid entry, False if we have reached the end</returns>
			public bool MoveNext()
			{
				// For the first entry, find the lowest valued node in the tree
				if (current == null)
					current = theTree.FindMostLeft(theTree.Root);
				else
				{
					// Can we go right-left?
					if (current.Right != null)
						current = theTree.FindMostLeft(current.Right);
					else
					{
						// Note the value we have found
						TKey CurrentValue = current.Data;

						// Go up the tree until we find a value larger than the largest we have
						// already found (or if we reach the root of the tree)
						while (current != null)
						{
							current = current.Parent;
							if (current != null)
							{
								int Compare = current.Data.CompareTo(CurrentValue);
								if (Compare < 0) continue;
							}
							break;
						}

					}
				}
				return (current != null);
			}

			public TKey Current
			{
				get
				{
					if (current == null)
						throw new InvalidOperationException();
					return current.Data;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (current == null)
						throw new InvalidOperationException();
					return current.Data;
				}
			}

			public void Dispose() { }
			public void Reset() { current = null; }
		}
	}
}

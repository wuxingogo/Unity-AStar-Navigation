//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
	List<KeyValuePair<T,int>> queue = new List<KeyValuePair<T,int>>();

	public void Enqueue(T key, int value)
	{
		queue.Add(new KeyValuePair<T,int>(key, value));
	}
	public T Dequeue()
	{
		KeyValuePair<T,int> keyPair = queue[0];
		for (int i = 1; i < queue.Count; i++) {
			if(keyPair.Value > queue[i].Value)
			{
				keyPair = queue[i];
			}
		}
		queue.Remove(keyPair);
		return keyPair.Key;
	}

	public int Count{
		get{
			return queue.Count;
		}
	}
}


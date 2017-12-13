using UnityEngine;
using System.Collections;
using System;
public class SingletonSimple<T> 
{

	protected static T instance = Activator.CreateInstance<T>();

	public static T getInstance
	{
		get
		{ 
			return instance;
		}
	}

	protected SingletonSimple()
	{

	}

}

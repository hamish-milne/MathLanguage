using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MathLanguage
{
	public delegate Value Operation<T1, T2>(T1 a, T2 b, bool assign);

	public class OperatorManager
	{
		public static OperatorManager I
		{
			get;
			set;
		}

		struct TypePair
		{
			Type a;
			Type b;

			public TypePair(Type a, Type b)
			{
				this.a = a;
				this.b = b;
			}

			public override int GetHashCode()
			{
				int x = a == null ? 0 : a.GetHashCode();
				int y = b == null ? 0 : b.GetHashCode();
				if (x > y)
					return (x * x) + x + y;
				return (y * y) + x;
			}

			public override bool Equals(object obj)
			{
				if(obj != null && obj is TypePair)
				{
					var other = (TypePair)obj;
					return other.a == a && other.b == b;
				}
				return false;
			}
		}

		readonly Dictionary<TypePair, Delegate>[] list;
		readonly MethodInfo genericMethod;
		readonly Dictionary<TypePair, DoOp> methodCache = new Dictionary<TypePair, DoOp>();
		static readonly Type[] typeParams = new Type[2];
		delegate Value DoOp(Operator op, Value a, Value b, bool assign);

		public OperatorManager()
		{
			var values = (Operator[])Enum.GetValues(typeof(Operator));
			var max = (int)values.Max((a, b) => (a > b));
			list = new Dictionary<TypePair, Delegate>[max + 1];
			genericMethod = ((DoOp)DoOperationNonGeneric<Value, Value>).Method;
		}

		public void Register<T1, T2>(Operator op, Operation<T1, T2> operation)
			where T1 : Value
			where T2 : Value
		{
			if (operation == null)
				throw new ArgumentException("operation");
			var index = (int)op;
			if (index < 0 || index >= list.Length)
				throw new ArgumentException("Invalid operator");
			var dict = list[index];
			if(dict == null)
			{
				dict = new Dictionary<TypePair, Delegate>();
				list[index] = dict;
			}
			dict[new TypePair(typeof(T1), typeof(T2))] = operation;
		}

		public Operation<T1, T2> Get<T1, T2>(Operator op)
			where T1 : Value
			where T2 : Value
		{
			var index = (int)op;
			if (index < 0 || index >= list.Length)
				throw new ArgumentException("Invalid operator");
			Delegate ret = null;
			var dict = list[index];
			if (dict != null)
				dict.TryGetValue(new TypePair(typeof(T1), typeof(T2)), out ret);
			return (Operation<T1, T2>)ret;
		}

		Value DoOperationNonGeneric<T1, T2>(Operator op, Value a, Value b, bool assign)
			where T1 : Value
			where T2 : Value
		{
			return DoOperation<T1, T2>(op, (T1)a, (T2)b, assign);
		}

		public Value DoOperation<T1, T2>(Operator op, T1 a, T2 b, bool assign)
			where T1 : Value
			where T2 : Value
		{
			if (a == null || b == null)
				throw new ArgumentNullException();
			var operation = Get<T1, T2>(op);
			if (operation == null)
				throw new InvalidOperatorException(op, a.TypeName, b.TypeName);
			return operation(a, b, assign);
		}

		public Value DoOperation(Operator op, Value a, Value b, bool assign)
		{
			if(a == null || b == null)
				throw new ArgumentNullException();
			DoOp func;
			typeParams[0] = a.GetType();
			typeParams[1] = a.GetType();
			var pair = new TypePair(typeParams[0], typeParams[1]);
			methodCache.TryGetValue(pair, out func);
			if(func == null)
			{
				func = (DoOp)Delegate.CreateDelegate(typeof(DoOp),
					genericMethod.MakeGenericMethod(typeParams));
				methodCache[pair] = func;
			}
			return func(op, a, b, assign);
		}
	}
}

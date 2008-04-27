//
// Unit Test for AvoidComplexMethods Rule.
//
// Authors:
//      Cedric Vivier <cedricv@neonux.com>
//
//      (C) 2008 Cedric Vivier
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.Maintainability;
using Mono.Cecil;

using NUnit.Framework;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;


namespace Test.Rules.Maintainability {

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ExpectedCCAttribute : Attribute
	{
		public ExpectedCCAttribute(int cc)
		{
			_cc = cc;
		}

		public int Value
		{
			get { return _cc; }
		}

		private int _cc = 0;
	}

	public class MethodsWithExpectedCC
	{
		[ExpectedCC(1)]
		public void Test1()
		{
			Console.Write("1");
		}

		[ExpectedCC(1)]
		public void TestLong1(int x, int y)
		{
			Console.Write("{0} {1} {2} {3}", x, x+0, (x+0>y), y);
			Console.Write("{3} {2} {1} {0}", x, x+1, (x+1>y), y);
			Console.Write("{3} {2} {1} {0}", x, x+2, (x+2>y), y);
			Console.Write("{0} {2} {1} {3}", x, x+3, (x+3>y), y);
			Console.Write("{0} {1} {3} {2}", x, x+4, (x+4>y), y);
			Console.Write("{3} {1} {2} {0}", x, x+5, (x+5>y), y);
			Console.Write("{3} {1} {2} {0}", x*x, (x+5)*x, (((x+5)*x)>y), y*y);
		}

		[ExpectedCC(2)]
		public object Test2(object x)
		{
			return x ?? new object()/*2*/;
		}

		[ExpectedCC(3)]
		public int TestTernary3(int x)
		{
			return (x == 0) ? -1/*2*/ : 1/*3*/;
		}

		[ExpectedCC(5)]
		public void TestSwitch5(int x)
		{
			switch (x)
			{
				case 1:
				case 2:
				case 3:
					Console.Write("abc");
					break;
				case 4:
					Console.Write("d");
					break;
				case 5:
					Console.Write("e");
					break;
				default:
					Console.Write("default");
					break;
			}
			Console.Write("oob");
		}

		[ExpectedCC(6)]
		public void TestSwitch6(int x)
		{
			Console.Write("oob");
			switch (x)
			{
				case 1:
					Console.Write("a");
					break;
				case 2:
					Console.Write("b");
					break;
				case 3:
					Console.Write("c");
					break;
				case 4:
					Console.Write("d");
					break;
				case 5:
					Console.Write("e");
					break;
			}
			Console.Write("oob");
		}

		[ExpectedCC(7)]
		public void TestSwitch7(int x)
		{
			switch (x)
			{
				case 1:
					Console.Write("a");
					break;
				case 2:
					Console.Write("b");
					break;
				case 3:
					Console.Write("c");
					break;
				case 4:
					Console.Write("d");
					break;
				case 5:
					Console.Write("e");
					break;
				default:
					Console.Write("default");
					break;
			}
			Console.Write("oob");
		}

		[ExpectedCC(6)]
		public object Test6(object val)
		{
			if (val != null) {/*2*/
				return (val == this) ? 1/*3*/ : 2/*4*/;
			}
			else if (val.GetType() == typeof(string))/*5*/
			{
				string sRef = val.GetType().FullName;
				return sRef ?? "foo"/*6*/;
			}
			throw new InvalidOperationException();
		}

		[ExpectedCC(14)]
		public void Test14(int x)
		{
			switch (x)
			{
				case 1:
					Console.Write("a");
					break;
				case 2:
					Console.Write("b");
					break;
				case 3:
					Console.Write("c");
					break;
				case 4:
					Console.Write("d");
					break;
				case 5:
					Console.Write("e");
					break;
				default:/*7*/
					Console.Write("default");
					break;
			}
			int y = this.GetHashCode();
			if (Math.Min(x, y) > 0/*8*/ && Math.Min(x,y) < 42/*9*/)
			{
				y = (x == 32) ? x/*10*/ : y/*11*/;
			}
			else if (x == 3000/*12*/ || y > 3000/*13*/ || y == 500/*14*/)
			{
				y = 200;
			}
			Console.Write("oob");
		}

		[ExpectedCC(15)]
		public object Test15(object val)
		{
			if (val == null) {/*2*/
				return null;
			}
			else if (val.GetType() == typeof(string))/*3*/
			{
				string sRef = "eateat";
				return sRef;
			}
			else if (val.GetType() == typeof(int)/*4*/
				|| val.GetType() == typeof(uint)/*5*/
				|| val.GetType() == typeof(float)/*6*/
				|| val.GetType() == typeof(double)/*7*/
				|| val.GetType() == typeof(byte)/*8*/
				|| val.GetType() == typeof(long)/*9*/
				|| val.GetType() == typeof(ulong)/*10*/
				|| val.GetType() == typeof(char))/*11*/
			{
				return 50;
			}
			else if (val.GetType() == typeof(bool))/*12*/
			{
				return (val.GetType().FullName == "foo") ? true/*13*/ : false/*14*/;
			}
			else if (val.GetType() == typeof(object))/*15*/
			{
				return val;
			}
			throw new InvalidOperationException();
		}

		[ExpectedCC(21)]
		public object Test21(object val)
		{
			if (val == null) {/*2*/
				return null;
			}
			else if (val.GetType() == typeof(string))/*3*/
			{
				string sRef = "eateat";
				return sRef;
			}
			else if (val.GetType() == typeof(int)/*4*/
				|| val.GetType() == typeof(uint)/*5*/
				|| val.GetType() == typeof(float)/*6*/
				|| val.GetType() == typeof(double)/*7*/
				|| val.GetType() == typeof(byte)/*8*/
				|| val.GetType() == typeof(long)/*9*/
				|| val.GetType() == typeof(ulong)/*10*/
				|| val.GetType() == typeof(char))/*11*/
			{
				return 50;
			}
			else if (val.GetType() == typeof(bool))/*12*/
			{
				return (val.GetType().FullName == "foo") ? true/*13*/ : false/*14*/;
			}
			else if (val.GetType() == typeof(object))/*15*/
			{
				return val;
			}
			int i = val.GetHashCode();
			if (i > 0/*16*/ && i < 42/*17*/)
			{
				return null;
			}
			else if (i == 42/*18*/ || i == 69/*19*/)
			{
				return (i == 42) ? true/*20*/ : false/*21*/;
			}
			throw new InvalidOperationException();
		}

		[ExpectedCC(15)]
		[System.Runtime.CompilerServices.CompilerGenerated]
		public object Generated15(object val)
		{
			if (val == null) {/*2*/
				return null;
			}
			else if (val.GetType() == typeof(string))/*3*/
			{
				string sRef = "eateat";
				return sRef;
			}
			else if (val.GetType() == typeof(int)/*4*/
				|| val.GetType() == typeof(uint)/*5*/
				|| val.GetType() == typeof(float)/*6*/
				|| val.GetType() == typeof(double)/*7*/
				|| val.GetType() == typeof(byte)/*8*/
				|| val.GetType() == typeof(long)/*9*/
				|| val.GetType() == typeof(ulong)/*10*/
				|| val.GetType() == typeof(char))/*11*/
			{
				return 50;
			}
			else if (val.GetType() == typeof(bool))/*12*/
			{
				return (val.GetType().FullName == "foo") ? true/*13*/ : false/*14*/;
			}
			else if (val.GetType() == typeof(object))/*15*/
			{
				return val;
			}
			throw new InvalidOperationException();
		}

	}


	[TestFixture]
	public class AvoidComplexMethodsTest : MethodRuleTestFixture<AvoidComplexMethodsRule>
	{

		[Test]
		public void CyclomaticComplexityMeasurementTest ()
		{
			Type rType = Assembly.GetExecutingAssembly ().GetType ("Test.Rules.Maintainability.MethodsWithExpectedCC");
			TypeDefinition type = DefinitionLoader.GetTypeDefinition<MethodsWithExpectedCC> ();
			ExpectedCCAttribute expectedCC;
			int cc;

			foreach (MethodDefinition method in type.Methods)
			{
				cc = AvoidComplexMethodsRule.GetCyclomaticComplexityForMethod(method);
				expectedCC = (ExpectedCCAttribute)
					rType.GetMethod (method.Name).GetCustomAttributes(
										typeof(ExpectedCCAttribute), false)[0];
				Assert.AreEqual (cc, expectedCC.Value,
					"CC for method '{0}' is {1} but should have been {2}.",
					method, cc, expectedCC);
			}
		}

		[Test]
		public void SimpleMethodsTest ()
		{
			AssertRuleSuccess<MethodsWithExpectedCC> ("Test1");
			AssertRuleSuccess<MethodsWithExpectedCC> ("TestLong1");
			AssertRuleSuccess<MethodsWithExpectedCC> ("Test2");
			AssertRuleSuccess<MethodsWithExpectedCC> ("TestTernary3");
			AssertRuleSuccess<MethodsWithExpectedCC> ("TestSwitch5");
			AssertRuleSuccess<MethodsWithExpectedCC> ("TestSwitch6");
			AssertRuleSuccess<MethodsWithExpectedCC> ("TestSwitch7");
			AssertRuleSuccess<MethodsWithExpectedCC> ("Test6");
			AssertRuleSuccess<MethodsWithExpectedCC> ("Test14");
		}

		[Test]
		public void ComplexMethodTest ()
		{
			AssertRuleFailure<MethodsWithExpectedCC> ("Test15");
		}

		[Test]
		public void MoreComplexMethodTest ()
		{
			AssertRuleFailure<MethodsWithExpectedCC> ("Test21");
		}

		[Test]
		public void MethodDoesNotApplyTest ()
		{
			AssertRuleDoesNotApply<MethodsWithExpectedCC> ("Generated15");
		}

	}

}


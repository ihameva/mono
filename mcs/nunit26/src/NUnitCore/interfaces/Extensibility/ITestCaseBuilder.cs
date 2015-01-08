// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Reflection;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// The ITestCaseBuilder interface is exposed by a class that knows how to
	/// build a test case from certain methods. 
	/// </summary>
	public interface ITestCaseBuilder
	{
        /// <summary>
        /// Examine the method and determine if it is suitable for
        /// this builder to use in building a TestCase.
        /// 
        /// Note that returning false will cause the method to be ignored 
        /// in loading the tests. If it is desired to load the method
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="method">The test method to examine</param>
        /// <returns>True is the builder can use this method</returns>
        bool CanBuildFrom(MethodInfo method);

        /// <summary>
        /// Build a TestCase from the provided MethodInfo.
        /// </summary>
        /// <param name="method">The method to be used as a test case</param>
        /// <returns>A TestCase or null</returns>
        Test BuildFrom(MethodInfo method);
    }

    /// <summary>
    /// ITestCaseBuilder2 extends ITestCaseBuilder with methods
    /// that include the suite for which the test case is being
    /// built. Test case builders not needing the suite can
    /// continue to implement ITestCaseBuilder.
    /// </summary>
    public interface ITestCaseBuilder2 : ITestCaseBuilder
    {
        /// <summary>
        /// Examine the method and determine if it is suitable for
        /// this builder to use in building a TestCase to be
        /// included in the suite being populated.
        /// 
        /// Note that returning false will cause the method to be ignored 
        /// in loading the tests. If it is desired to load the method
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="method">The test method to examine</param>
        /// <param name="suite">The suite being populated</param>
        /// <returns>True is the builder can use this method</returns>
        bool CanBuildFrom(MethodInfo method, Test suite);

        /// <summary>
        /// Build a TestCase from the provided MethodInfo for
        /// inclusion in the suite being constructed.
        /// </summary>
        /// <param name="method">The method to be used as a test case</param>
        /// <param name="suite">The test suite being populated, or null</param>
        /// <returns>A TestCase or null</returns>
        Test BuildFrom(MethodInfo method, Test suite);
    }
}

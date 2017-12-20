using ApprovalTests.Combinations;
using ApprovalTests.Reporters;
using NUnit.Framework;
using StatePrinting;

namespace Emails
{
	[TestFixture]
	public class NewRateEmailMessager_Should
	{
		private NewRateEmailMessager messager;
		private Stateprinter printer;

		[SetUp]
		public void SetUp()
		{
			messager = new NewRateEmailMessager();
			printer = new Stateprinter();
			printer.Configuration.Add(new StringBuilderConverter());
		}

		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void TestCreation_When_IncreaseRate_Is_True()
		{
			Config.Local.IncreaseRate = true;
			CreateMessageAndTest();
		}

		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void TestCreation_When_IncreaseRate_Is_False()
		{
			Config.Local.IncreaseRate = false;
			CreateMessageAndTest();
		}

		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void TestCreation_With_DifferentRates2()
		{
			Config.Local.IncreaseRate = true;
			Config.Local.IncreaseRateFactor = 2;
			CreateMessageAndTest();
		}
		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void TestCreation_With_DifferentRates1()
		{
			Config.Local.IncreaseRate = true;
			Config.Local.IncreaseRateFactor = 1;
			CreateMessageAndTest();
		}
		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void TestCreation_With_DifferentRates0()
		{
			Config.Local.IncreaseRate = true;
			Config.Local.IncreaseRateFactor = 0;
			CreateMessageAndTest();
		}

		private void CreateMessageAndTest()
		{
			CombinationApprovals.VerifyAllCombinations(
				(name, type, newRate) => printer.PrintObject(messager.CreateMessage(name, type, newRate)),
				new[] { "Olga", "Evgeniy" },
				new[] { AccountType.Cheque, AccountType.Credit, AccountType.Savings },
				new[] { 0.1m, 0.01m, 0.2m });
		}
	}
}
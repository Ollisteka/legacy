using System.Collections.Generic;

namespace Emails
{
	public class NewRateEmailMessager
	{
		private readonly Dictionary<AccountType, string> respect = new Dictionary<AccountType, string>()
		{
			[AccountType.Cheque] = "chequing account",
			[AccountType.Credit] = "credit card",
			[AccountType.Savings] = "on line savings account",
		};
		private readonly Dictionary<AccountType, string> newRates = new Dictionary<AccountType, string>()
		{
			[AccountType.Cheque] = "The interest rate at which you earn interest has changed to {0}%.",
			[AccountType.Credit] = "The interest rate for which you will be charged for new purchases is now {0}%",
			[AccountType.Savings] = "Your savings interest rate has changed to {0}%",
		};
		public EmailMessage CreateMessage(string customerName, AccountType accountType, decimal newRate)
		{
			return CreateMessage(customerName, accountType, newRate, Config.Local);
		}

		public EmailMessage CreateMessage(string customerName, AccountType accountType, decimal newRate, Config config)
		{
			var message = new EmailMessage();

			message.Subject = "New rate!";
			var sb = message.Body;

			sb.AppendFormatLine("Dear {0}", customerName);
			sb.AppendLine();

			sb.Append("We are sending you this message with respect to your ");
			sb.Append(respect[accountType]);

			sb.AppendLine();
			sb.AppendLine();


			if (config.IncreaseRate)
			{
				newRate *= config.IncreaseRateFactor;
			}
			sb.AppendFormatLine(newRates[accountType], newRate);

			if (newRate > 0.1m)
				message.Important = true;

			sb.AppendLine();
			sb.AppendLine();

			sb.AppendLine("Kind regards - your bank.");
			return message;
		}
	}
}
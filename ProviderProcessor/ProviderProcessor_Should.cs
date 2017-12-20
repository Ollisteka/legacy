using System;
using ApprovalTests;
using ApprovalTests.Combinations;
using ApprovalTests.Reporters;
using FakeItEasy;
using Newtonsoft.Json;
using NUnit.Framework;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;
using ProviderProcessing.References;

namespace ProviderProcessing
{
	[TestFixture]
	[UseReporter(typeof(DiffReporter))]
	public class ProviderProcessor_Should
	{
		private ProviderData data;
		private string dataSerialized;
		private IProviderRepository repository;
		private ProductsReference productsReference;
		private MeasureUnitsReference measureUnitsReference;
		private string measurecode = "abc";
		private string productName = "abc";


		[SetUp]
		public void SetUp()
		{
			data = new ProviderData();
			data.Products = new ProductData[0];
			dataSerialized = JsonConvert.SerializeObject(data);
			repository = A.Fake<IProviderRepository>();
			A.CallTo(() => repository.FindByProviderId(A<Guid>.Ignored)).Returns(null);
			A.CallTo(() => repository.RemoveById(A<Guid>.Ignored)).DoesNothing();
			A.CallTo(() => repository.Save(A<ProviderData>.Ignored)).DoesNothing();
			A.CallTo(() => repository.Update(A<ProviderData>.Ignored)).DoesNothing();

			productsReference = A.Fake<ProductsReference>();
			A.CallTo(() => productsReference.FindCodeByName(A<string>.Ignored)).Returns(null);

			measureUnitsReference = A.Fake<MeasureUnitsReference>();
			A.CallTo(() => measureUnitsReference.FindByCode(A<string>.Ignored)).Returns(null);
		}

		[Test]
		public void NoDataOnServer_NoProductData()
		{
			var provider = new ProviderProcessor(productsReference, measureUnitsReference, repository);
			Approvals.Verify(provider.ProcessProviderData(dataSerialized));
		}

		[Test]
		public void DoSomething_WhenSomething()
		{
			var serverData = new ProviderData();
			serverData.Timestamp = DateTime.Parse("01.01.01");
			serverData.Products = new[] { new ProductData() { Id = new Guid(), MeasureUnitCode = "", Name = "", Price = 10 } };
			CombinationApprovals.VerifyAllCombinations(
				Verify,
				new string[]{measurecode},
				new string[] {productName},
				new decimal[] {10},
				new ProviderData[] {null, data, serverData},
				new int?[] {null, 11},
				new MeasureUnit[] {new MeasureUnit(), null},
				new DateTime[] {DateTime.MinValue, DateTime.MaxValue, DateTime.Parse("01.01.01"), }
				);
		}

		private ProcessReport Verify(string measureCode, string name, decimal price, ProviderData dataOnServer, int? code, MeasureUnit measureUnit, DateTime timestamp)
		{
			data.Timestamp = timestamp;
			data.Products = new[] { new ProductData() { Id = new Guid(), MeasureUnitCode = measureCode, Name = name, Price = price} };
			dataSerialized = JsonConvert.SerializeObject(data);
			A.CallTo(() => repository.FindByProviderId(A<Guid>.Ignored)).Returns(dataOnServer);
			A.CallTo(() => productsReference.FindCodeByName(A<string>.Ignored)).Returns(code);
			A.CallTo(() => measureUnitsReference.FindByCode(A<string>.Ignored)).Returns(measureUnit);
			return new ProviderProcessor(productsReference, measureUnitsReference, repository).ProcessProviderData(dataSerialized);
		}

		[Test]
		public void NoDataOnServer_CorrectData()
		{
			data.Products = new []{new ProductData(){Id=new Guid(), MeasureUnitCode = measurecode, Name = productName, Price = 10} };
			dataSerialized = JsonConvert.SerializeObject(data);
			A.CallTo(() => productsReference.FindCodeByName(productName)).Returns(0);
			A.CallTo(() => measureUnitsReference.FindByCode(measurecode)).Returns(new MeasureUnit());
			var provider = new ProviderProcessor(productsReference, measureUnitsReference, repository);
			//provider.ProcessProviderData(dataSerialized);
			Approvals.Verify(provider.ProcessProviderData(dataSerialized));
		}
	}
}
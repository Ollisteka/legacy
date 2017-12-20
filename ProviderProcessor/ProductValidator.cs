using System.Collections.Generic;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;
using ProviderProcessing.References;

namespace ProviderProcessing
{
	public class ProductValidator
	{
		private readonly ProductsReference productsReference;
		private readonly MeasureUnitsReference measureUnitsReference;

		public ProductValidator(ProductsReference productsReference, MeasureUnitsReference measureUnitsReference)
		{
			this.productsReference = productsReference;
			this.measureUnitsReference = measureUnitsReference;
		}

		public IList<ProductValidationResult> ValidateProduct(ProductData product)
		{
			var result = new List<ProductValidationResult>();

			if (!productsReference.FindCodeByName(product.Name).HasValue)
				result.Add(new ProductValidationResult(product,
					"Unknown product name", ProductValidationSeverity.Error));

			if (product.Price <= 0)
				result.Add(new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning));

			if (!IsValidMeasureUnitCode(product.MeasureUnitCode))
				result.Add(new ProductValidationResult(product,
					"Bad units of measure", ProductValidationSeverity.Warning));

			return result;
		}

		private bool IsValidMeasureUnitCode(string measureUnitCode)
		{
			return measureUnitsReference.FindByCode(measureUnitCode) != null;
		}
	}
}
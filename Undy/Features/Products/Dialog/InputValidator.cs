namespace Undy.Features.Products.Dialog
{
    public sealed class ProductInput
    {
        public string ProductNumber { get; init; } = "";
        public string ProductName { get; init; } = "";
        public string Size { get; init; } = "";
        public string Colour { get; init; } = "";
        public decimal Price { get; init; }
        public int NumberInStock { get; init; }
    }

    public static class ProductInputValidator
    {
        public static bool TryValidate(
            string? productNumberRaw,
            string? productNameRaw,
            string? priceRaw,
            string? sizeRaw,
            string? colourRaw,
            string? stockRaw,
            out ProductInput? input,
            out string error)
        {
            input = null;
            error = "";

            var productNumber = (productNumberRaw ?? "").Trim();
            var productName = (productNameRaw ?? "").Trim();
            var size = (sizeRaw ?? "").Trim();
            var colour = (colourRaw ?? "").Trim();


            // If Product Number is missing
            if (string.IsNullOrWhiteSpace(productNumber))
            {
                error = "Produktnummer skal udfyldes.";
                return false;
            }

            // If Product Name is missing
            if (string.IsNullOrWhiteSpace(productName))
            {
                error = "Produktnavn skal udfyldes.";
                return false;
            }

            // If Price is missing or not a number
            if (!TryParseDecimal(priceRaw, out var price))
            {
                error = "Pris skal være et gyldigt tal.";
                return false;
            }

            // If Price is negative (who wants to give away money and merchandise?)
            if (price < 0)
            {
                error = "Prisen kan ikke være negativ.";
                return false;
            }


            // If Amount is missing or not an integer
            if (!int.TryParse((stockRaw ?? "").Trim(),
                              NumberStyles.Integer,
                              CultureInfo.CurrentCulture,
                              out var stock))
            {
                error = "Antal på lager skal være et gyldigt heltal.";
                return false;
            }

            // If Size is missing
            if (string.IsNullOrWhiteSpace(size))
            {
                error = "Størrelse skal udfyldes.";
                return false;
            }

            // If colour is missing
            if (string.IsNullOrWhiteSpace(colour))
            {
                error = "Farve skal udfyldes.";
                return false;
            }

            input = new ProductInput
            {
                ProductNumber = productNumber,
                ProductName = productName,
                Price = price,
                Size = size,
                Colour = colour,
                NumberInStock = stock
            };

            return true;
        }

        private static bool TryParseDecimal(string? raw, out decimal value)
        {
            var s = (raw ?? "").Trim();

            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
                return true;

            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }
    }
}

using FluentValidation;
using FluentValidation.Results;
using System.Globalization;
using Test.Application.DTO.Transaction;
using Test.Application.Helper;
using Test.Application.Services.IServices;
using Test.Core.Domain;
using Test.Core.IRepository;

namespace Test.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IValidator<TransactionInputDTO> _transactionValidator;

        public TransactionService
        (
            IPartnerRepository partnerRepository,
            IValidator<TransactionInputDTO> transactionValidator
        )
        {
            _partnerRepository = partnerRepository;
            _transactionValidator = transactionValidator;
        }

        public async Task<TransactionOutputDTO> SubmitTransactionAsync(TransactionInputDTO input)
        {
            try
            {
                TransactionOutputDTO output = new TransactionOutputDTO();

                #region Validation
                // Validate Common Business Logics
                ValidationResult validationResult = await _transactionValidator.ValidateAsync(input);
                if (!validationResult.IsValid)
                {
                    output.Result = 0;
                    output.ResultMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
                    return output;
                }

                // Validate Signature
                if (!ValidateSignature(input))
                {
                    output.Result = 0;
                    output.ResultMessage = "Access Denied!";
                    return output;
                }

                // Validate Partner
                if (!await ValidatePartnerAsync(input))
                {
                    output.Result = 0;
                    output.ResultMessage = "Access Denied!";
                    return output;
                }

                // Validate TimeStamp
                if (!ValidateTimestamp(input.Timestamp))
                {
                    output.Result = 0;
                    output.ResultMessage = "Expired!";
                    return output;
                }
                #endregion

                #region Process
                int discount = 0;
                int totalAmount = (int)input.TotalAmount / 100; // convert to RM

                switch (totalAmount)
                {
                    case var amount when amount < 200:
                        break;

                    case var amount when amount >= 200 && amount <= 500:
                        discount = 5;
                        break;

                    case var amount when amount >= 501 && amount <= 800:
                        discount = 7;
                        break;

                    case var amount when amount >= 801 && amount <= 1200:
                        discount = 10;
                        break;

                    case var amount when amount > 1200:
                        discount = 15;
                        break;
                }

                switch (totalAmount)
                {
                    case var amount when amount > 500 && IsPrime(amount):
                        discount += 8;
                        break;

                    case var amount when amount > 900 && (amount % 10 == 5):
                        discount += 10;
                        break;
                }

                // Discount capped at 20%
                discount = discount > 20 ? 20 : discount;

                output.Result = 1;
                output.TotalAmount = input.TotalAmount;
                output.TotalDiscount = (long)(input.TotalAmount * (discount / 100.0));
                output.FinalAmount = input.TotalAmount - output.TotalDiscount;
                #endregion

                return output;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        #region Validation
        private bool ValidateSignature(TransactionInputDTO input)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(input.Timestamp, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                string formattedTimestamp = parsedDate.ToString("yyyyMMddHHmmss");

                string signature = formattedTimestamp + input.PartnerKey + input.PartnerRefNo + input.TotalAmount.ToString() + input.PartnerPassword;

                string hashValue = HashHelper.ComputeHash(signature);

                string encodedHash = EncodingHelper.EncodeToBase64(hashValue);

                return encodedHash == input.Sig;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> ValidatePartnerAsync(TransactionInputDTO input)
        {
            List<Partner> allowedPartners = await _partnerRepository.GetAllAsync();

            return allowedPartners.Any(p => p.PartnerKey == input.PartnerKey &&
                                            p.PartnerRefNo == input.PartnerRefNo &&
                                            EncodingHelper.EncodeToBase64(p.PartnerPassword) == input.PartnerPassword);
        }

        private bool ValidateTimestamp(string timestamp)
        {
            try
            {
                DateTime serverTimeUtc = DateTime.UtcNow;

                DateTime providedTimestampUtc = DateTime.ParseExact(timestamp, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                TimeSpan timeDifference = serverTimeUtc - providedTimestampUtc;

                if (Math.Abs(timeDifference.TotalMinutes) <= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
    }
}

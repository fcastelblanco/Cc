using System.Collections.Generic;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Implementations
{
    public class ValidateService : IValidateService
    {
        private readonly Dictionary<FileTypeValidate, IValidateStrategy> _validateStrategies;

        public ValidateService()
        {
            _validateStrategies = new Dictionary<FileTypeValidate, IValidateStrategy>
            {
                  {FileTypeValidate.WebAplication, new ValidateStrategy.WebFolderValidate()},
                  {FileTypeValidate.Services, new ValidateStrategy.ValidateFolder()},
                  {FileTypeValidate.ScriptCommit, new ValidateStrategy.ValidateFolder()},
                  {FileTypeValidate.ScriptRollback, new ValidateStrategy.ValidateFolder()},
                  {FileTypeValidate.Library, new ValidateStrategy.LibraryValidateFolder()},
                  {FileTypeValidate.Templates, new ValidateStrategy.TemplatesValidateFolder()},
                  {FileTypeValidate.KnowledgeBank, new ValidateStrategy.KnowledgeBankValidateFolder()},
                  {FileTypeValidate.AttachedBank, new ValidateStrategy.AttachedBankValidateFolder()}
           };
        }

        public bool Validate(FileTypeValidate fileType, string input,bool backup)
        {
            return _validateStrategies[fileType].Validate(input, backup);
        }
    }
}

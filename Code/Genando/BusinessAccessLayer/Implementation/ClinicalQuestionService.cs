using BusinessAccessLayer.Abstraction;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Implementation;

public class ClinicalQuestionService :
    GenericService<ClinicalQuestion>, IClinicalQuestionService
{
    #region Constructors
    private readonly IUnitOfWork _unitOfWork;

    public ClinicalQuestionService(IClinicalQuestionRepository repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #endregion Constructors

    #region Interface Methods

    public async Task<IEnumerable<int>> GetAllQuestions()
    {
        IEnumerable<ClinicalQuestion> questions = await GetAllAsync(select: QuesionIdSelect);
        return questions.Select(question => question.Id);
    }

    #endregion Interface Methods

    #region Helper Filters

    private static Expression<Func<ClinicalQuestion, ClinicalQuestion>> QuesionIdSelect
        => question => new()
        {
            Id = question.Id
        };

    #endregion
}
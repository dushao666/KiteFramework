namespace Application.Validations;

/// <summary>
/// 登录参数校验器
/// </summary>
public class SignInValidator : AbstractValidator<SignInCommand>
{
    public SignInValidator()
    {
        RuleFor(x => x.Type).NotEmpty().WithMessage("登录类型不能为空").DependentRules(() =>
        {
            RuleFor(x => x.Type).IsInEnum().WithMessage("登录类型错误");
        });
        RuleFor(x => x.Login).NotEmpty().When(x => x.Type == LoginType.Password).WithMessage("登录名不能为空");
        RuleFor(x => x.Password).NotEmpty().When(x => x.Type == LoginType.Password).WithMessage("密码不能为空");
        RuleFor(x => x.DingTalkAuthCode).NotEmpty().When(x => x.Type == LoginType.DingTalk).WithMessage("钉钉免登授权码不能为空");
    }
}

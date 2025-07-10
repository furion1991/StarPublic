import cn from 'classnames'

import { Button } from '@/shared/ui'

import { authModalBaseClasses } from '@/entities/auth'
import classes from './EmailVerificationSuccess.module.scss'

type EmailVerificationSuccessProps = {
  onNextStep: () => void
}

export const EmailVerificationSuccess = ({ onNextStep }: EmailVerificationSuccessProps) => {
  return (
    <div>
      <h4 className={authModalBaseClasses.heading}>
        Почта успешно подтверждена, <br /> можете выполнить вход в аккаунт
      </h4>

      <Button
        type='button'
        className={cn(authModalBaseClasses.submitBtn, classes.btn)}
        onClick={onNextStep}
      >
        Продолжить
      </Button>
    </div>
  )
}

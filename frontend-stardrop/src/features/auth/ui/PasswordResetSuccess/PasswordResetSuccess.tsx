import cn from 'classnames'

import { Button } from '@/shared/ui'

import { authModalBaseClasses } from '@/entities/auth'
import classes from './PasswordResetSuccess.module.scss'

type PasswordResetSuccessProps = {
  onStepBack: () => void
}

export const PasswordResetSuccess = ({ onStepBack }: PasswordResetSuccessProps) => {
  return (
    <div className={classes.passwordResetSuccess}>
      <h4 className={authModalBaseClasses.heading}>Пароль успешно сброшен</h4>

      <p>Отправили новый пароль на вашу почту</p>

      <Button
        type='button'
        className={cn(authModalBaseClasses.submitBtn, classes.btn)}
        onClick={onStepBack}
      >
        Спасибо
      </Button>
    </div>
  )
}

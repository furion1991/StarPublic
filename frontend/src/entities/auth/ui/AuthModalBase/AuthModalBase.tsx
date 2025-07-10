import Image from 'next/image'

import { Modal } from '@/shared/ui'

import classes from './AuthModalBase.module.scss'

import CrossIcon from '@/public/icons/cross.svg'
import ArrowLeftIcon from '@/public/icons/arrow-left.svg'

type AuthModalBaseProps = {
  children: React.ReactNode
  open: boolean
  SocialAuthSlot: React.ReactNode
  stepBackAvailable?: boolean
  onStepBack?: () => void
  onClose: () => void
}

export const AuthModalBase = ({
  children,
  open,
  SocialAuthSlot,
  stepBackAvailable,
  onStepBack = () => {},
  onClose
}: AuthModalBaseProps) => {
  return (
    <Modal className={classes.modal} open={open} onClose={onClose}>
      {stepBackAvailable ? (
        <button type='button' className={classes.backBtn} onClick={onStepBack}>
          <ArrowLeftIcon />
        </button>
      ) : null}

      <div className={classes.logo}>
        <Image src='/img/logo-pink.svg' width={156} height={155} alt='Логотип' />
      </div>

      <button type='button' className={classes.closeBtn} onClick={onClose}>
        <CrossIcon />
      </button>

      <div className={classes.content}>
        {children}

        <div className={classes.socialAuth}>
          <p>Войти с помощью:</p>

          <div className={classes.socialNetworsList}>{SocialAuthSlot}</div>
        </div>

        <p className={classes.agreement}>
          Прочел и согласен с Политика конфиденциальности и Пользовательское соглашение
        </p>
      </div>
    </Modal>
  )
}

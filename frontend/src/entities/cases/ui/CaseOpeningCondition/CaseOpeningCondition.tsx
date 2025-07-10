import Image from 'next/image'
import cn from 'classnames'
import Link from 'next/link'

import { Button, PriceWithCurrency } from '@/shared/ui'

import { useAuthModal, useUser } from '@/shared/hooks'

import classes from './CaseOpeningCondition.module.scss'

type Condition = 'not-auth' | 'not-enough-balance'

type CaseOpeningConditionProps = {
  condition: Condition
  caseOpenPrice: number
}

export const CaseOpeningCondition = ({ condition, caseOpenPrice }: CaseOpeningConditionProps) => {
  const { openAuthModal } = useAuthModal()
  const { user } = useUser()

  const userBalance = user?.currentBalance || 0
  const howMuchBalanceNotEnough = caseOpenPrice - userBalance

  return (
    <div className={classes.caseOpeningCondition}>
      {condition === 'not-auth' ? (
        <>
          <div className={cn(classes.condition, classes.notAuth)}>
            <p className={classes.uppercase}>Вы не авторизованы!</p>
            <p>Авторизуйтесь для открытия кейсов</p>
          </div>

          <Button boxShadow onClick={openAuthModal}>
            Войти
          </Button>
        </>
      ) : null}

      {condition === 'not-enough-balance' ? (
        <>
          <div className={cn(classes.condition, classes.notEnoughtBalance)}>
            <p>
              Не хватает <PriceWithCurrency>{howMuchBalanceNotEnough}</PriceWithCurrency>
            </p>

            <p>Недостаточно средств для открытия кейса</p>
          </div>

          <Link href='/deposit' className={classes.balanceUpLink}>
            <Button color='purple'>
              <Image src='/icons/wallet-filled.svg' width={22} height={18} alt='Кошелёк' />{' '}
              Пополнить баланс
            </Button>
          </Link>
        </>
      ) : null}
    </div>
  )
}

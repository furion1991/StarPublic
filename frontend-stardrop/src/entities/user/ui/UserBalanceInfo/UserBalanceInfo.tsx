import Image from 'next/image'
import Link from 'next/link'

import { Button, PriceWithCurrency } from '@/shared/ui'

import { useUser } from '@/shared/hooks'

import classes from './UserBalanceInfo.module.scss'

export const UserBalanceInfo = () => {
  const { user } = useUser()

  return (
    <div className={classes.userBalanceInfo}>
      <div className={classes.left}>
        <div className={classes.row}>
          <div className={classes.currencyIcon}>
            <Image src='/icons/wallet-gradient.svg' width={30} height={30} alt='Кошелёк' />
          </div>

          <PriceWithCurrency
            className={classes.balance}
            image={{
              width: 26,
              height: 26
            }}
          >
            {user ? new Intl.NumberFormat('de-DE').format(user?.currentBalance) : 0}
          </PriceWithCurrency>
        </div>

        <div className={classes.row}>
          <div className={classes.currencyIcon}>
            <Image src='/img/gem.png' width={22} height={24} alt='Гем' />
          </div>

          <span>0</span>
        </div>
      </div>

      <Link href='/deposit' className={classes.balanceUpLink}>
        <Button color='purple' borderRadius='medium'>
          Пополнить ›
        </Button>
      </Link>
    </div>
  )
}

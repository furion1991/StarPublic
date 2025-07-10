import Image from 'next/image'

import classes from './WithdrawItem.module.scss'

type Status = 'success' | 'in-process' | 'canceled'

type WithdrawItemProps = {
  status: Status
}

export const WithdrawItem = ({ status }: WithdrawItemProps) => {
  return (
    <div className={classes.withdrawItem}>
      <div className={classes.itemImage}>
        <Image src='/placeholders/case-loot.png' width={113} height={57} alt='Оружие' />

        <div className={classes.rarity}>
          <Image src='/img/loot-rarity-circle.svg' width={109} height={109} alt='Редкость' />
        </div>
      </div>

      <p className={classes.itemName}>Starned Corbem</p>

      <p className={classes.price}>294</p>

      <div className={classes.status}>
        {status === 'success' ? (
          <Image src='/icons/checkmark-circle.svg' width={21} height={21} alt='Успешно' />
        ) : null}

        {status === 'in-process' ? (
          <Image src='/icons/clock-circle.svg' width={21} height={21} alt='В процессе' />
        ) : null}

        {status === 'canceled' ? (
          <Image src='/icons/cross-circle.svg' width={21} height={21} alt='Отменено' />
        ) : null}
      </div>
    </div>
  )
}

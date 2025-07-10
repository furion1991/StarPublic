import Image from 'next/image'

import classes from './BonusItem.module.scss'

export const BonusItem = () => {
  return (
    <div className={classes.bonusItem}>
      <Image src='/placeholders/bonus-item.png' width={170} height={168} alt='Бонус' />

      <p>2 бесплатных кейса после пополнения от 400</p>
    </div>
  )
}

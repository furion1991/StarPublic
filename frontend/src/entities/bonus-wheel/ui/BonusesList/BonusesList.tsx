import Image from 'next/image'
import classes from './BonusesList.module.scss'

export const BonusesList = () => {
  const bonuses = Array.from({ length: 7 })
    .fill(null)
    .map((_, idx) => {
      return (
        <li key={idx} className={classes.bonusItem}>
          <Image src='/placeholders/bonus-wheel-loot.png' width={224} height={236} alt='Бонус' />
        </li>
      )
    })

  return <ul>{bonuses}</ul>
}

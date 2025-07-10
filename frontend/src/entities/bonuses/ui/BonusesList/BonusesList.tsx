import Image from 'next/image'
import classes from './BonusesList.module.scss'
import { Button } from '@/shared/ui'

export const BonusesList = () => {
  const bonuses = [
    {
      label: 'Ежедневный бонус',
      text: 'Ежедневный бонус от 1 до 2500 рублей каждые 24 часа',
      img: {
        src: '/img/bonus-time.png',
        width: 136,
        height: 131
      }
    },
    {
      label: 'Разовый бонус',
      text: 'Разовый бонус дается за выполнение списказаданий',
      img: {
        src: '/img/bonus-quest.png',
        width: 160,
        height: 149
      }
    },
    {
      label: 'Бонус «розыгрыш»',
      text: 'Розыгрыш от 100 до 500 бонусов',
      img: {
        src: '/img/bonus-chest.png',
        width: 129,
        height: 146
      }
    }
  ]

  return (
    <ul className={classes.bonusesList}>
      {bonuses.map(({ label, text, img }) => {
        return (
          <li key={label}>
            <h4>{label}</h4>

            <p>{text}</p>

            <Button>Открыть ›</Button>

            <Image
              className={classes.bonusImg}
              src={img.src}
              width={img.width}
              height={img.height}
              alt={label}
            />
          </li>
        )
      })}
    </ul>
  )
}

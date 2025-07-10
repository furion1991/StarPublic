import Image from 'next/image'
import Link from 'next/link'

import classes from './NavList.module.scss'

export const NavList = () => {
  const navList = [
    {
      label: 'Кейсы',
      href: '/',
      image: (
        <Image
          src='/icons/treasure-chest.svg'
          width={27}
          height={24}
          alt='Сундук с сокровищами'
          priority
        />
      )
    },
    {
      label: 'Контракты',
      href: '/contracts',
      image: <Image src='/icons/medal.svg' width={21} height={21} alt='Медаль' priority />
    },
    {
      label: 'Апгрейд',
      href: '/upgrades',
      image: (
        <Image src='/icons/upgrade-double.svg' width={22} height={22} alt='Улучшение' priority />
      )
    },
    {
      label: 'Барабан',
      href: '/bonus-wheel',
      image: <Image src='/icons/drum-wheel.svg' width={19} height={22} alt='Барабан' priority />
    },
    {
      label: 'Бонусы',
      href: '/bonuses',
      image: <Image src='/icons/gift-box.svg' width={22} height={22} alt='Подарок' priority />
    }
  ]

  return (
    <nav>
      <ul className={classes.navList}>
        {navList.map(({ label, href, image }) => {
          return (
            <li key={label}>
              <Link href={href}>
                <span className={classes.icon}>{image}</span>

                <span className={classes.label}>{label}</span>
              </Link>
            </li>
          )
        })}
      </ul>
    </nav>
  )
}

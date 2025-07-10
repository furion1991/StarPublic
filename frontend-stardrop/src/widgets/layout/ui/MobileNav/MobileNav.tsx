'use client'

import Image from 'next/image'
import classes from './MobileNav.module.scss'
import Link from 'next/link'
import dynamic from 'next/dynamic'
const Popup = dynamic(() => import('reactjs-popup'), { ssr: false })

import { NavPopup } from '@/shared/ui'
import { useAuth } from '@/shared/hooks'

export const MobileNav = () => {
  const { logout } = useAuth()

  const nav = [
    { label: 'Кейсы', href: '/', iconSrc: '/icons/mobileNav/treasure-chest.svg' },
    // { label: 'Краш', href: '/crash-rocket', iconSrc: '/icons/mobileNav/rocket.svg' },
    { label: 'Бонусы', href: '/bonuses', iconSrc: '/icons/mobileNav/gift-box.svg' },
    // { label: 'Барабан', href: '/bonus-wheel', iconSrc: '/icons/mobileNav/drum-wheel.svg' },
    { label: 'Меню', iconSrc: '/icons/mobileNav/hamburger.svg' }
  ]

  return (
    <ul className={classes.mobileNav}>
      {nav.map(({ label, href, iconSrc }) => {
        if (!href) {
          return (
            <li key={label}>
              <Popup
                className={classes.userPopup}
                arrow={false}
                closeOnDocumentClick
                position='bottom right'
                overlayStyle={{
                  display: 'none'
                }}
                trigger={
                  <button>
                    <Image src={iconSrc} width={36} height={36} alt={label} />
                    <span>{label}</span>
                  </button>
                }
              >
                {/* @ts-expect-error: Skip lib type */}
                {(close) => {
                  return (
                    <NavPopup
                      className={classes.mobileNavPopup}
                      onClose={close}
                      onLogout={logout}
                    />
                  )
                }}
              </Popup>
            </li>
          )
        }

        return (
          <li key={label}>
            <Link href={href}>
              <Image src={iconSrc} width={36} height={36} alt={label} />
              <span>{label}</span>
            </Link>
          </li>
        )
      })}
    </ul>
  )
}

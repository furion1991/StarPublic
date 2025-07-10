'use client'

import Link from 'next/link'
import Image from 'next/image'
import cn from 'classnames'

import { CrashRocketLink } from '../CrashRocketLink/CrashRocketLink'
import { useAuth } from '@/shared/hooks'

import classes from './NavPopup.module.scss'
import LogoutIcon from '@/public/icons/exit.svg'

type NavPopupProps = {
  className?: string
  onClose?: () => void
  onLogout: () => void
}

export const NavPopup = ({ className, onLogout, onClose }: NavPopupProps) => {
  const { isAuth } = useAuth()

  return (
    <div className={cn(classes.navPopup, className)}>
      {/* <div onClick={onClose}>
        <CrashRocketLink className={classes.crashRocketLink} rounded />
      </div> */}

      <ul onClick={onClose}>
        {isAuth ? (
          <>
            <li>
              <Link href='/profile'>
                <Image src='/icons/user-rounded.svg' width={20} height={20} alt='Пользователь' />
                <span>Профиль</span>
              </Link>
            </li>

            <li>
              <Link href='/deposit'>
                <Image src='/icons/wallet.svg' width={20} height={20} alt='Кошелек' />
                <span>Пополнить баланс</span>
              </Link>
            </li>
          </>
        ) : null}

        <li className={classes.mobileLinks}>
          <Link href='/upgrades'>
            <Image src='/icons/upgrade-mini-white.svg' width={20} height={20} alt='Кошелек' />
            <span>Апгрейд</span>
          </Link>
        </li>

        <li className={classes.mobileLinks}>
          <Link href='/contracts'>
            <Image src='/icons/medal-mini-white.svg' width={20} height={20} alt='Кошелек' />
            <span>Контракты</span>
          </Link>
        </li>

        <li>
          <Link href='/faq'>
            <Image src='/icons/info-rounded.svg' width={20} height={20} alt='Информация' />
            <span>F.A.Q</span>
          </Link>
        </li>

        {isAuth ? (
          <li>
            <button type='button' onClick={onLogout}>
              <LogoutIcon />
              <span>Выйти</span>
            </button>
          </li>
        ) : null}
      </ul>

      <svg
        className={classes.triangle}
        xmlns='http://www.w3.org/2000/svg'
        width='25'
        height='16'
        viewBox='0 0 25 16'
        fill='none'
      >
        <path
          d='M11.7389 15.107C12.1382 15.5755 12.8618 15.5755 13.2611 15.107L24.5183 1.89865C25.0716 1.24943 24.6102 0.25 23.7572 0.25H1.2428C0.38977 0.25 -0.0716038 1.24943 0.48172 1.89865L11.7389 15.107Z'
          fill='#0F164B'
        />
      </svg>
    </div>
  )
}

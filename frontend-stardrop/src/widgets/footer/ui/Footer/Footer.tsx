import Link from 'next/link'
import Image from 'next/image'

import { Button, Logo, SocialNetworksLinksList } from '@/shared/ui'

import classes from './Footer.module.scss'

export const Footer = () => {
  const nav = [
    { label: 'Пользовательское соглашение', href: '/user-agreement' },
    { label: 'Контакты', href: '/' },
    { label: 'Соглашение о приватности', href: '/privacy-policy' },
    { label: 'Вопрос - ответ', href: '/faq' }
  ]

  return (
    <footer className={classes.footer}>
      <div className={classes.topLine}>
        <ul className={classes.navList}>
          {nav.map(({ label, href }) => {
            return (
              <li key={label}>
                <Link href={href}>{label}</Link>
              </li>
            )
          })}
        </ul>

        <div className={classes.topRight}>
          <div className={classes.logo}>
            <Logo />
            <span>StarDrop</span>
          </div>

          <div className={classes.socialNetworksList}>
            <SocialNetworksLinksList />
          </div>
        </div>
      </div>

      <div className={classes.bottomLine}>
        {/*<p className={classes.companyCredentialsText}>*/}
        {/*  Company number 13246765, 16 John Nicholas Crescent, Ellesmere Port, Cheshire, United*/}
        {/*  Kingdom, CH65 2DL, +44 07308278693.*/}
        {/*</p>*/}

        <Button className={classes.collaborationBtn} borderRadius='medium'>
          <Image src='/icons/handshake.svg' width={24} height={24} alt='рукопожатие' />{' '}
          Сотрудничество
        </Button>
      </div>

      <ul className={classes.navListMobile}>
        {nav.map(({ label, href }) => {
          return (
            <li key={label}>
              <Link href={href}>{label}</Link>
            </li>
          )
        })}
      </ul>

      {/*<p className={classes.companyCredentialsTextMobile}>*/}
      {/*  Company number 13246765, 16 John Nicholas Crescent, Ellesmere Port, Cheshire, United*/}
      {/*  Kingdom, CH65 2DL, +44 07308278693.*/}
      {/*</p>*/}

      <div className={classes.bgCharacter}>
        <Image src='/img/clash-of-clans-guy.png' width={492} height={300} alt='Персонаж' />
      </div>
    </footer>
  )
}

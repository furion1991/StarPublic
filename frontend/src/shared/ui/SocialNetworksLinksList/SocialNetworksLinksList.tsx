import Image from 'next/image'
import Link from 'next/link'

import classes from './SocialNetworksLinksList.module.scss'

export const SocialNetworksLinksList = () => {
  const socialNetworksList = [
    {
      label: 'tg', // Удалить после добавления ссылки, используется для key
      href: '/',
      icon: <Image src='/social/telegram.svg' width={20} height={17} alt='Телеграм' priority />
    },
    {
      label: 'vk', // Удалить после добавления ссылки, используется для key
      href: '/',
      icon: <Image src='/social/vk.svg' width={24} height={13} alt='Вконтакте' priority />
    },
    {
      label: 'discord', // Удалить после добавления ссылки, используется для key
      href: '/',
      icon: <Image src='/social/discord.svg' width={23} height={15} alt='Дискорд' priority />
    }
  ]

  return (
    <ul className={classes.socialNetworksLinksList}>
      {socialNetworksList.map(({ label, href, icon }) => {
        return (
          <li key={label}>
            <Link href={href} className={classes.link}>
              {icon}
            </Link>
          </li>
        )
      })}
    </ul>
  )
}

'use client'

import Image from 'next/image'
import Link from 'next/link'

type LogoProps = {
  withShadow?: boolean
}

export const Logo = ({ withShadow = false }: LogoProps) => {
  return (
    <Link
      href='/'
      style={{
        display: 'flex'
      }}
    >
      <Image
        quality={100}
        src='/icons/logo.svg'
        width={64}
        height={63}
        style={
          withShadow
            ? {
                filter: 'drop-shadow(0px 0px 9.1px rgba(255, 255, 255, 0.35))'
              }
            : {}
        }
        alt='StarDrop лого'
      />
    </Link>
  )
}

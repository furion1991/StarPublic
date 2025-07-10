import Image from 'next/image'
import cn from 'classnames'

import classes from './PriceWithCurrency.module.scss'

type PriceWithCurrencyProps = {
  children: React.ReactNode
  className?: string
  image?: {
    width?: number
    height?: number
  }
}

export const PriceWithCurrency = ({ children, className, image }: PriceWithCurrencyProps) => {
  return (
    <span className={cn(classes.priceWithCurrency, className)}>
      {children}{' '}
      <Image
        src='/icons/logo-mini.svg'
        width={image?.width || 20}
        height={image?.height || 20}
        alt='Валюта'
      />
    </span>
  )
}

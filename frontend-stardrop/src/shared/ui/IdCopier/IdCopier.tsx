import Image from 'next/image'
import cn from 'classnames'

import classes from './IdCopier.module.scss'

type IdCopierProps = {
  id: string
  className?: string
}

export const IdCopier = ({ id, className }: IdCopierProps) => {
  const copyIdToClipboard = (id: string) => {
    navigator.clipboard.writeText(id)
  }

  return (
    <div
      className={cn(classes.idCopier, className)}
      onClick={() => {
        copyIdToClipboard(id)
      }}
    >
      ID: <span>{id}</span>
      <button type='button'>
        <Image src='/img/copy.png' width={12} height={12} alt='Копировать' />{' '}
      </button>
    </div>
  )
}
